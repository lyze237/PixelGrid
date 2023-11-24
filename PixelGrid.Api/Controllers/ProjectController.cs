﻿using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using PixelGrid.Api.Data;
using PixelGrid.Api.Filters;
using PixelGrid.Api.Helpers;
using PixelGrid.Api.Managers;

namespace PixelGrid.Api.Controllers;

public record ProjectIndexModel(List<Project> OwnProjects, List<Project> SharedProjects);

[Authorize]
public class ProjectController(ApplicationDbContext dbContext, UserManager<User> userManager, ChunkManager chunkManager, ILogger<ProjectController> logger) : Controller
{
    private static readonly FormOptions DefaultFormOptions = new();

    public async Task<IActionResult> Index()
    {
        var user = await userManager.GetUserAsync(User) ?? throw new ArgumentException("User is null?");

        var model = new ProjectIndexModel(
            await dbContext.Projects
                .Include(c => c.SharedWith)
                .Where(c => c.Owner == user)
                .ToListAsync(),
            await dbContext.Projects
                .Include(c => c.Owner)
                .Where(c => c.SharedWith.Contains(user))
                .ToListAsync()
        );
        return View(model);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return BadRequest($"{nameof(name)} is not set.");

        var user = await userManager.GetUserAsync(User) ?? throw new ArgumentException("User is null?");

        dbContext.Projects.Add(new Project(name, user.Id));
        await dbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest("No id given.");

        var user = await userManager.GetUserAsync(User) ?? throw new ArgumentException("User is null?");
        var project = await dbContext.Projects
            .Include(c => c.SharedWith)
            .FirstOrDefaultAsync(c => c.Id == id && c.Owner == user);

        if (project == null)
            return BadRequest("Id not found or not owner.");

        return View(project);
    }

    public async Task<IActionResult> Delete(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest("No id given.");

        var user = await userManager.GetUserAsync(User) ?? throw new ArgumentException("User is null?");
        var project = await dbContext.Projects
            .Include(c => c.SharedWith)
            .FirstOrDefaultAsync(c => c.Id == id && c.Owner == user);

        if (project == null)
            return BadRequest("Id not found or not owner.");

        dbContext.Projects.Remove(project);
        await dbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [RequestSizeLimit(10000000)]
    [HttpPost]
    [DisableFormValueModelBinding]
    public async Task<IActionResult> Upload()
    {
        if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            return BadRequest("Not a valid form.");
        
        var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(Request.ContentType), DefaultFormOptions.MultipartBoundaryLengthLimit);
        var reader = new MultipartReader(boundary, HttpContext.Request.Body);

        var chunk = new Chunk();
        await chunk.Parse(reader);

        if (!chunk.ValidFileName())
            return BadRequest("Invalid filename");

        if (!chunk.ValidFilePath())
            return BadRequest("Invalid filepath");
        
        chunkManager.StoreChunk(chunk);

        if (!chunkManager.IsValid(chunk.Uuid))
            return BadRequest("Chunk upload received garbled data.");

        if (chunkManager.IsComplete(chunk.Uuid))
        {
            await chunkManager.Save(@"C:\\temp\\uploads", chunk.Uuid);
            return Ok("Completed!~");
        }

        return Ok();
    }
}