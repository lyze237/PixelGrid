using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using PixelGrid.Server.Domain.Repositories;
using PixelGrid.Server.Services;
using PixelGrid.Shared.Hubs;

namespace PixelGrid.Server.Hubs;

/// <summary>
/// Represents a SignalR hub for rendering functionalities.
/// </summary>
/// <typeparam name="IRenderHub.IClient">The client interface for the RenderHub.</typeparam>
[Authorize(Policy = "RenderClient")]
public class RenderHub(RenderClientsManagementService renderManagementService, ILogger<RenderHub> logger) : Hub<IRenderHub.IClient>, IRenderHub.IServer
{
    /// <summary>
    /// Sends a message from the client to the server in the RenderHub.
    /// </summary>
    /// <param name="message">The message to be sent from the client to the server.</param>
    public async Task ClientToServer(string message)
    {
        logger.LogInformation("Received {Message} from client", message);
        await Clients.All.ServerToClient($"Pong {message}");
    }

    public override async Task OnConnectedAsync() => 
        await renderManagementService.SetClientConnectionStatus(Context.User, true);

    public override async Task OnDisconnectedAsync(Exception? exception) => 
        await renderManagementService.SetClientConnectionStatus(Context.User, false);
}