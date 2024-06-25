namespace PixelGrid.Renderer.abstracts;

public abstract class Options
{
    public int Width { get; set; } = 1920;
    public int Height { get; set; } = 1080;

    public bool UseBorder { get; set; } = false;
    public float? BorderMinX { get; private set; }
    public float? BorderMinY { get; private set; }
    public float? BorderMaxX { get; private set; }
    public float? BorderMaxY { get; private set; }

    public int StartFrame { get; set; } = 1;
    public int? EndFrame { get; set; } = null;

    public Options WithBorder(float minX, float minY, float maxX = 1, float maxY = 1)
    {
        UseBorder = true;
        
        BorderMinX = minX;
        BorderMinY = minY;
        BorderMaxX = maxX;
        BorderMaxY = maxY;

        return this;
    }
}
