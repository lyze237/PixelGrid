using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using PixelGrid.Server.Database;
using PixelGrid.Server.Database.Entities;
using PixelGrid.Server.Database.Repositories;
using PixelGrid.Server.Hubs;
using PixelGrid.Server.Options;
using PixelGrid.Shared.Hubs;

namespace PixelGrid.Server.Services;

public class RenderJobManagementService(
    ApplicationDbContext dbContext,
    IOptions<RendererOptions> options,
    RenderClientProgramVersionRepository programVersionRepository,
    IHubContext<RenderHub, IRenderHub.IClient> renderHub,
    ILogger<RenderJobManagementService> logger)
{
    public async Task ForceStartProject(ProjectEntity project)
    {
        var team = project.Workspace.Team;
        var renderClients = dbContext.TeamRenderClients
            .Where(rc => rc.Team == team)
            .Select(rc => rc.RenderClient)
            .ToList();
        
        foreach (var renderClient in renderClients
                     .Select(renderClientEntity => renderHub.Clients.Client(renderClientEntity.ConnectionId!)))
            await renderClient.AssignProject(project.Id);
    }
}