using PixelGrid.Renderer.abstracts;

namespace PixelGrid.Renderer.povray;

public class PovrayCallbackStatus : CallbackStatus
{
    public long? CurrentPixel { get; set; }
    public long? TotalPixels { get; set; }
}