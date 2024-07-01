using PixelGrid.Shared.Renderer;

namespace PixelGrid.Client.Options;

public class RendererOptions
{
    public string Workdir { get; set; }
    public string Url { get; set; }
    public string Token { get; set; }
    
    public Dictionary<RenderType, List<RenderVersionMapping>> Versions { get; set; } = new();

    public record RenderVersionMapping(string Version, string Exe);
}
