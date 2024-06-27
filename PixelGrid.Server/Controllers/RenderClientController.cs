using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using PixelGrid.Server.Services;

namespace PixelGrid.Server.Controllers;

/// <summary>
/// Represents a controller for managing client operations.
/// </summary>
public class RenderClientController(ClientService clientService, ILogger<RenderClientController> logger) : RenderClientControllerProto.RenderClientControllerProtoBase
{
    /// <summary>
    /// Registers a client.
    /// </summary>
    /// <param name="request">The registration request.</param>
    /// <param name="context">The server call context.</param>
    /// <returns>The registration response.</returns>
    [Authorize]
    public override async Task<RenderClientRegisterResponse> Register(RenderClientRegisterRequest request,
        ServerCallContext context) =>
        await clientService.Register(request);
}