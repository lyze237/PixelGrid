using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace PixelGrid.Server.Hubs;

[Authorize]
public class ClientHub(ILogger<ClientHub> logger) : Hub
{
    public async Task Login()
    {
        logger.LogInformation("Received login");
        await Clients.All.SendAsync("ReceiveMessage", "Hey");
    }
}