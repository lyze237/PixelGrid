namespace PixelGrid.Renderer.abstracts;

public interface IRenderer
{
    void Render(string workingDirectory, string filename, string outputDirectory, string outputFilename, Options options, IRenderCallback callback);

    RendererCapabilities GetCapabilities();
}