using FluentResults.Extensions.AspNetCore;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using PixelGrid.Server.Services;
using PixelGrid.Shared.Models.Controller;

namespace PixelGrid.Server.Controllers;

/// <summary>
/// Represents a controller for managing client operations.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class RenderClientController(RenderClientsManagementService renderClientsManagementService, ILogger<RenderClientController> logger) : ControllerBase
{
    /// <summary>
    /// Registers a client.
    /// </summary>
    /// <param name="request">The registration request.</param>
    /// <param name="context">The server call context.</param>
    /// <returns>The registration response.</returns>
    [Authorize]
    public async Task<ActionResult> Register(RenderClientRegisterRequest request, ServerCallContext context) =>
        await renderClientsManagementService.Register(request).ToActionResult();
}