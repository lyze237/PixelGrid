using Microsoft.AspNetCore.SignalR;
using PixelGrid.Server.Hubs;
using PixelGrid.Shared.Hubs;

namespace PixelGrid.Server.Services;

public class RenderJobManagementService(
    IHubContext<RenderHub, IRenderHub.IClient> renderHub,
    ILogger<RenderJobManagementService> logger)
{
    public async Task StartTestJob()
    {
        throw new NotImplementedException();
    }
}