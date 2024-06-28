using System.Security.Cryptography;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using PixelGrid.Server.Services;

namespace PixelGrid.Server.Controllers;

/// <summary>
/// Represents a controller for managing render jobs.
/// </summary>
[Authorize]
public class RenderJobController(RenderJobManagementService renderJobManagementService, ILogger<RenderJobController> logger) : RenderJobControllerProto.RenderJobControllerProtoBase
{
    /// <summary>
    /// Starts the render test.
    /// </summary>
    /// <param name="request">The start render test request.</param>
    /// <param name="context">The server call context.</param>
    /// <returns>The start render test response.</returns>
    public override async Task<StartRenderTestResponse> StartTestRender(StartRenderTestRequest request, ServerCallContext context)
    {
        await renderJobManagementService.StartTestJob();
        return new StartRenderTestResponse();
    }
}