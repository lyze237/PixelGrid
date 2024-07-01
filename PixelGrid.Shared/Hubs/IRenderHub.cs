using PixelGrid.Shared.Renderer;
using PixelGrid.Shared.Renderer.Options;

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

        Task AssignProject(long projectId);
    }
}