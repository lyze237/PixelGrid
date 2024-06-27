namespace PixelGrid.Shared.Hubs;

public interface IRenderHub
{
    public interface IServer
    {
        Task ClientToServer(string message);
    }

    public interface IClient
    {
        Task ServerToClient(string message);

        Task ForceDisconnect(string reason);
    }
}