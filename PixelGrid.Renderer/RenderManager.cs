using PixelGrid.Renderer.abstracts;
using PixelGrid.Renderer.blender;
using PixelGrid.Renderer.povray;

namespace PixelGrid.Renderer;

public class RenderManager
{
    public Dictionary<string, IRenderer> RegisteredRenderers { get; } = new()
    {
        {"povray", new PovrayRenderer()},
        {"blender", new BlenderRenderer()}
    };

    public RenderManager()
    {
    }

    public void Render(string name, string workingDirectory, string filename, string outputDirectory, string outputFilename, Options options, IRenderCallback callback) => 
        RegisteredRenderers[name].Render(workingDirectory, filename, outputDirectory, outputFilename, options, callback);
}