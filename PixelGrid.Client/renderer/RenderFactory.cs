using PixelGrid.Client.renderer.abstracts;
using PixelGrid.Shared.Renderer;

namespace PixelGrid.Client.renderer;

public class RenderFactory(IServiceProvider services)
{
    public IRenderer GetRenderer(RenderType type, string exe)
    {
        var renderer = services.GetRequiredKeyedService<IRenderer>(type);
        renderer.ExePath = exe;

        return renderer;
    }
}