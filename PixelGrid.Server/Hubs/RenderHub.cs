using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using PixelGrid.Server.Domain.Repositories;
using PixelGrid.Server.Services;
using PixelGrid.Shared.Hubs;
using PixelGrid.Shared.Renderer;

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

    /// <summary>
    /// Registers a program with the RenderHub.
    /// </summary>
    /// <param name="type">The type of the renderer program. Possible values are Povray or Blender.</param>
    /// <param name="version">The version of the renderer program.</param>
    /// <param name="rendererCapabilities">The capabilities of the renderer program.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task RegisterProgram(RenderType type, string version, RendererCapabilities rendererCapabilities) =>
        await renderManagementService.RegisterProgram(Context, type, version, rendererCapabilities);

    /// <summary>
    /// Overrides the OnConnectedAsync method in the SignalR Hub class and handles the logic when a client connects to the RenderHub.
    /// </summary>
    /// <param name="exception">The exception that occurred during the client connection, if any.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public override async Task OnConnectedAsync() => 
        await renderManagementService.ClientRegistered(Context);

    /// <summary>
    /// Handles the asynchronous event when a client is disconnected from the RenderHub.
    /// </summary>
    /// <param name="exception">The exception that caused the disconnection, if any.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public override async Task OnDisconnectedAsync(Exception? exception) =>
        await renderManagementService.ClientDisconnected(Context);
}