using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using PixelGrid.Server.Services;
using PixelGrid.Shared.Hubs;
using PixelGrid.Shared.Renderer;

namespace PixelGrid.Server.Hubs;

[Authorize(Policy = "RenderClient")]
public class RenderHub(RenderClientsManagementService renderManagementService, ILogger<RenderHub> logger) : Hub<IRenderHub.IClient>, IRenderHub.IServer
{
    public async Task ClientToServer(string message)
    {
        logger.LogInformation("Received {Message} from client", message);
        await Clients.All.ServerToClient($"Pong {message}");
    }

    public async Task RegisterProgram(RenderType type, string version, RendererCapabilities rendererCapabilities) =>
        await renderManagementService.RegisterProgram(Context, type, version, rendererCapabilities);

    public override async Task OnConnectedAsync() => 
        await renderManagementService.ClientRegistered(Context);

    public override async Task OnDisconnectedAsync(Exception? exception) =>
        await renderManagementService.ClientDisconnected(Context);
}