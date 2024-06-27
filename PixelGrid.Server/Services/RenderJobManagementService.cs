using Microsoft.AspNetCore.SignalR;
using PixelGrid.Server.Domain.Entities;
using PixelGrid.Server.Domain.Repositories;
using PixelGrid.Server.Hubs;
using PixelGrid.Shared.Hubs;
using PixelGrid.Shared.Renderer;

namespace PixelGrid.Server.Services;

public class RenderJobManagementService(
    IRenderClientRepository renderClientRepository,
    IRenderClientProgramVersionRepository programVersionRepository,
    IHubContext<RenderHub, IRenderHub.IClient> renderHub,
    ILogger<RenderJobManagementService> logger)
{
    public async Task StartTestJob(string workspace)
    {
        var clients = programVersionRepository
            .GetAvailableClients(RenderType.Povray, "3.7");
        
        logger.LogInformation("Starting render job with {Clients} clients", clients.Count);

        // TODO Mark as unavailable
        SendWorkspaceList(workspace, "Ansichten_2.pov", RenderType.Povray, clients);
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