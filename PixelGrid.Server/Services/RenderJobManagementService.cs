using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using PixelGrid.Server.Domain.Entities;
using PixelGrid.Server.Domain.Repositories;
using PixelGrid.Server.Hubs;
using PixelGrid.Server.Options;
using PixelGrid.Shared.Hubs;
using PixelGrid.Shared.Renderer;

namespace PixelGrid.Server.Services;

/// <summary>
/// Service for managing render jobs.
/// </summary>
public class RenderJobManagementService(
    IOptions<RendererOptions> options,
    IRenderClientProgramVersionRepository programVersionRepository,
    IHubContext<RenderHub, IRenderHub.IClient> renderHub,
    ILogger<RenderJobManagementService> logger)
{
    /// <summary>
    /// Starts a test render job.
    /// </summary>
    /// <param name="workspace">The path to the workspace directory.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task StartTestJob()
    {
        var clients = programVersionRepository
            .GetAvailableClients(RenderType.Povray, "3.7");
        
        logger.LogInformation("Starting render job with {Clients} clients", clients.Count);

        // TODO Mark as unavailable
        SendWorkspaceList(options.Value.Workdir, "Ansichten_2.pov", RenderType.Povray, clients);
    }

    private void SendWorkspaceList(string workspace, string filename, RenderType type, List<RenderClientEntity> clients)
    {
        var files = Directory
            .GetFiles(workspace, "*", SearchOption.AllDirectories)
            .Select(d => d[(workspace.Length + 1)..]).ToArray();
        
        logger.LogInformation("Sending {Files} files to all clients", files.Length);
        
        foreach (var clientEntity in clients)
        {
            var renderClient = renderHub.Clients.Client(clientEntity.ConnectionId!);
            
            logger.LogInformation("Sending {Files} to client {Client}", files.Length, clientEntity.Name);
            renderClient.CreateJob(1, files, filename, type);
        }
    }
}