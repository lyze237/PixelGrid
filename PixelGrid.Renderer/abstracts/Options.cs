namespace PixelGrid.Renderer.abstracts;

public abstract class Options
{
    public int Width { get; set; } = 1920;
    public int Height { get; set; } = 1080;

    public int StartFrame { get; set; } = 1;
    public int? EndFrame { get; set; } = null;
}
