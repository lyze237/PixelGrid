using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PixelGrid.Api.Data;

namespace PixelGrid.Api.Controllers;

public record ProjectIndexModel(List<Project> OwnProjects, List<Project> SharedProjects);

[Authorize]
public class ProjectController(ApplicationDbContext dbContext, UserManager<User> userManager) : Controller
{
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
    public async Task<IActionResult> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return BadRequest($"{nameof(name)} is not set.");
        
        var user = await userManager.GetUserAsync(User) ?? throw new ArgumentException("User is null?");

        dbContext.Projects.Add(new Project(name, user.Id));
        await dbContext.SaveChangesAsync();
        
        return RedirectToAction(nameof(Index));
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
}