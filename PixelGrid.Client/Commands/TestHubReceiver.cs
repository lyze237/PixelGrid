using PixelGrid.Shared.Hubs;

namespace PixelGrid.Client.Commands;

public class TestHubReceiver(ILogger<TestHubReceiver> logger) : ITestHub.ITestHubClient
{
    public Task ReceiveMessage(string user, string message)
    {
        logger.LogInformation("{user}: {message}", user, message);
        return Task.CompletedTask;
    }
}