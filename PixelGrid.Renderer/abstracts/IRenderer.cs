namespace PixelGrid.Renderer.abstracts;

public interface IRenderer
{
    void Render(string workingDirectory, string filename, string outputDirectory, Options options, IRenderCallback callback);
}