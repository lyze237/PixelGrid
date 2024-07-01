using PixelGrid.Shared.Renderer;
using PixelGrid.Shared.Renderer.Options;

namespace PixelGrid.Client.renderer.abstracts;

public interface IRenderer
{
    public string ExePath { get; set; }
    
    void Render(string workingDirectory, string filename, string outputDirectory, string outputFilename, RenderOptions options, IRenderCallback callback);

    RendererCapabilities GetCapabilities();
}