using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using PixelGrid.Server.Services;

namespace PixelGrid.Server.Controllers;

/// <summary>
/// Represents a controller for managing client operations.
/// </summary>
public class ClientController(ClientService clientService, ILogger<ClientController> logger) : Client.ClientBase
{
    /// <summary>
    /// Registers a client.
    /// </summary>
    /// <param name="request">The registration request.</param>
    /// <param name="context">The server call context.</param>
    /// <returns>The registration response.</returns>
    [Authorize]
    public override async Task<ClientRegisterResponse> Register(ClientRegisterRequest request,
        ServerCallContext context) =>
        await clientService.Register(request);
}