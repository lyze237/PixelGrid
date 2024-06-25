using PixelGrid.Renderer.abstracts;
using PixelGrid.Renderer.blender;

namespace PixelGrid.Renderer.Callbacks;

public class StdoutCallback : IRenderCallback
{
    public void OnStart() => 
        Console.WriteLine("[Callback] Render Started");

    public void OnProgress(CallbackStatus status)
    {
        Console.WriteLine("[Callback] " + status switch
        {
            BlenderCallbackStatus blenderStatus =>
                $"Frame: {blenderStatus.Frame}, Sample {blenderStatus.CurrentSample} / {blenderStatus.TotalSamples}, Memory: {blenderStatus.MemoryUsage} / {blenderStatus.PeakMemoryUsage}, Remaining Time: {blenderStatus.RemainingTime}",
            _ => $"Frame: {status.Frame}"
        });
    }

    public void OnWarning(string warning) => 
        Console.WriteLine($"[Callback] Warning: {warning}");

    public void OnError(string error) => 
        Console.WriteLine($"[Callback] Error: {error}");

    public void OnProgress(int frame, int samples, int totalSamples) => 
        Console.WriteLine($"[Callback] Render update {frame}: {samples} / {totalSamples}");

    public void OnCompleted() => 
        Console.WriteLine("[Callback] Render Completed");

    public void OnLog(string line) => 
        Console.Error.WriteLine($"[Callback] {line}");
}