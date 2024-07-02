using FluentResults.Extensions.AspNetCore;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using PixelGrid.Server.Services;
using PixelGrid.Shared.Models.Controller;

namespace PixelGrid.Server.Controllers;

[Route("Api/[controller]")]
[ApiController]
public class RenderClientController(RenderClientsManagementService renderClientsManagementService, ILogger<RenderClientController> logger) : ControllerBase
{
    [Authorize]
    [HttpPost("Register")]
    public async Task<ActionResult> Register(RenderClientRegisterRequest request) =>
        await renderClientsManagementService.Register(request).ToActionResult();
}