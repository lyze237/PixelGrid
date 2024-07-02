using FluentResults.Extensions.AspNetCore;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using PixelGrid.Server.Services;
using PixelGrid.Shared.Models.Controller;

namespace PixelGrid.Server.Controllers;

[Route("Api/[controller]")]
[ApiController]
public class AuthController(UserManagementService userManagement, ILogger<AuthController> logger) : ControllerBase
{
    [HttpPost("Register")]
    public async Task<ActionResult> Register(AuthRegisterRequest request) =>
        await userManagement.Register(request).ToActionResult();

    [HttpGet("Login")]
    public async Task<ActionResult> Login([FromQuery] AuthLoginRequest request) => 
        await userManagement.Login(request).ToActionResult();
}