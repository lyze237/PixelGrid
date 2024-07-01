namespace PixelGrid.Shared.Renderer.Options;

public abstract class RenderOptions
{
    public int Width { get; set; } = 1920;
    public int Height { get; set; } = 1080;

    public int Frame { get; set; } = 0;
    
    public AnimationOptions? Animation { get; set; }

    public BorderOptions? Border { get; set; } = null;
}

public class AnimationOptions(int startFrame, int endFrame)
{
    public int StartFrame => startFrame;
    public int EndFrame => endFrame;
}

public class BorderOptions(float? borderMinX, float? borderMinY, float? borderMaxX = 1, float? borderMaxY = 1)
{
    public float? BorderMinX => borderMinX;
    public float? BorderMinY => borderMinY;
    public float? BorderMaxX => borderMaxX;
    public float? BorderMaxY => borderMaxY;
}