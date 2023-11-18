namespace PixelGrid.Shared.Hubs;

public interface ITestHub
{
    public interface ITestHubServer
    {
        Task SendMessage(string user, string message);
    }

    public interface ITestHubClient
    {
        Task ReceiveMessage(string user, string message);
    }
}