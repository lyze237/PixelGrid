using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using PixelGrid.Shared.Hubs;

namespace PixelGrid.Server.Hubs;

[Authorize]
public class RenderHub(ILogger<RenderHub> logger) : Hub<IRenderHub.IClient>, IRenderHub.IServer
{
    public async Task ClientToServer(string message)
    {
        logger.LogInformation("Received {message} from client", message);
        await Clients.All.ServerToClient($"Pong {message}");
    }
}