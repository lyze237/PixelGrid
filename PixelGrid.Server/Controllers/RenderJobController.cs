using Grpc.Core;
using PixelGrid.Server.Services;

namespace PixelGrid.Server.Controllers;

/// <summary>
/// Represents a controller for managing render jobs.
/// </summary>
public class RenderJobController(RenderJobManagementService renderJobManagementService) : RenderJobControllerProto.RenderJobControllerProtoBase
{
    /// <summary>
    /// Starts the render test.
    /// </summary>
    /// <param name="request">The start render test request.</param>
    /// <param name="context">The server call context.</param>
    /// <returns>The start render test response.</returns>
    public override async Task<StartRenderTestResponse> StartTestRender(StartRenderTestRequest request, ServerCallContext context)
    {
        return new StartRenderTestResponse();
    }
}