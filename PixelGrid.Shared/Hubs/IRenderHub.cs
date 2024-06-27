using PixelGrid.Shared.Renderer;

namespace PixelGrid.Shared.Hubs;

public interface IRenderHub
{
    public interface IServer
    {
        Task ClientToServer(string message);
        Task RegisterProgram(RenderType type, string version, RendererCapabilities rendererCapabilities);
    }

    public interface IClient
    {
        Task ServerToClient(string message);

        Task ForceDisconnect(string reason);
        Task CreateJob(long id, string[] filePaths, string projectFilename, RenderType type);
    }
}