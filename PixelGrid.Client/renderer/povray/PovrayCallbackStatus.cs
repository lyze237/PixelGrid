using PixelGrid.Client.renderer.abstracts;

namespace PixelGrid.Client.renderer.povray;

public class PovrayCallbackStatus : CallbackStatus
{
    public long? CurrentPixel { get; set; }
    public long? TotalPixels { get; set; }
}