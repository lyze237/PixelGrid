using PixelGrid.Renderer.abstracts;

namespace PixelGrid.Renderer.blender;

public class BlenderCallbackStatus : CallbackStatus
{
    public int? Frame { get; set; }
        
    public int? CurrentSample { get; set; }
    public int? TotalSamples { get; set; }

    public string? RemainingTime { get; set; }

    public float? MemoryUsage { get; set; }
    public float? PeakMemoryUsage { get; set; }
}