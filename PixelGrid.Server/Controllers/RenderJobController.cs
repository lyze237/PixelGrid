using FluentResults;
using FluentResults.Extensions.AspNetCore;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixelGrid.Server.Database;
using PixelGrid.Server.Database.Entities;
using PixelGrid.Server.Services;
using PixelGrid.Shared.Models.Controller;
using PixelGrid.Shared.Renderer.Options;

namespace PixelGrid.Server.Controllers;

[Authorize]
[Route("Api/[controller]")]
[ApiController]
public class RenderJobController(ApplicationDbContext dbContext, RenderJobManagementService renderJobManagementService, ILogger<RenderJobController> logger) : ControllerBase
{
    [HttpGet("StartTestRender")]
    public async Task<ActionResult> StartTestRender([FromQuery] StartRenderTestRequest request)
    {
        dbContext.Teams.RemoveRange(dbContext.Teams);
        var team = dbContext.Teams.Add(new TeamEntity
        {
            Name = "Test",
            Description = "Testifficate"
        }).Entity;

        var user = dbContext.Users.First();
        
        dbContext.TeamMembers.RemoveRange(dbContext.TeamMembers);
        dbContext.TeamMembers.Add(new TeamMemberEntity
        {
            Team = team,
            User = user,
            Permissions = UserPermission.Admin
        });

        var client = dbContext.RenderClients.First();
        
        dbContext.TeamRenderClients.RemoveRange(dbContext.TeamRenderClients);
        dbContext.TeamRenderClients.Add(new TeamRenderClientEntity
        {
            Team = team,
            RenderClient = client
        });

        var workspace = dbContext.Workspaces.Add(new WorkspaceEntity
        {
            Id = 584,
            Team = team,
            Name = "Test",
            Description = "Testifficate"
        }).Entity;

        dbContext.Projects.RemoveRange(dbContext.Projects);
        var project = dbContext.Projects.Add(new ProjectEntity
        {
            Name = "Ansicht 2",
            CreatedAt = DateTime.UtcNow,
            RenderFilePath = "Ansichten_2.pov",
            Options = new PovrayOptions
            {
                Width = 1920,
                Height = 1080,
            },
            Workspace = workspace
        }).Entity;

        await dbContext.SaveChangesAsync();

        await renderJobManagementService.ForceStartProject(project);

        return Result.Ok(new StartRenderTestResponse()).ToActionResult();
    }
}