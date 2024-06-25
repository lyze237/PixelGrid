using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace PixelGrid.Server.Hubs;

public class ChatHub(ILogger<ChatHub> logger) : Hub
{
    [Authorize]
    public async Task SendMessage(string user, string message)
    {
        logger.LogInformation("Received message, sending to all users {user}: {message}", user, message);
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}