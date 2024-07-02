using FluentResults.Extensions.AspNetCore;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using PixelGrid.Server.Services;
using PixelGrid.Shared.Models.Controller;

namespace PixelGrid.Server.Controllers;

/// <summary>
/// Class responsible for user authentication and authorization.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AuthController(UserManagementService userManagement, ILogger<AuthController> logger) : ControllerBase
{
    /// <summary>
    /// Registers a user.
    /// </summary>
    /// <param name="request">The registration request.</param>
    /// <param name="context">The server call context.</param>
    /// <returns>The registration response.</returns>
    public async Task<ActionResult> Register(AuthRegisterRequest request, ServerCallContext context) =>
        await userManagement.Register(request).ToActionResult();

    /// <summary>
    /// Logs in a user.
    /// </summary>
    /// <param name="request">The login request.</param>
    /// <param name="context">The server call context.</param>
    /// <returns>The login response.</returns>
    public async Task<ActionResult> Login(AuthLoginRequest request, ServerCallContext context) => 
        await userManagement.Login(request).ToActionResult();
}