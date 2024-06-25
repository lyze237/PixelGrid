using PixelGrid.Renderer.abstracts;
using PixelGrid.Renderer.blender;
using PixelGrid.Renderer.povray;

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
            PovrayCallbackStatus povrayStatus => $"Pixel: {povrayStatus.CurrentPixel} / {povrayStatus.TotalPixels}",
            _ => throw new ArgumentException($"Missing {nameof(CallbackStatus)} implementation", nameof(status))
        });
    }

    public void OnWarning(string warning) =>
        Console.WriteLine($"[Callback] Warning: {warning}");

    public void OnError(string error) =>
        Console.WriteLine($"[Callback] Error: {error}");

    public void OnCompleted(int processExitCode) =>
        Console.WriteLine("[Callback] Render Completed");

    public void OnLog(string line) =>
        Console.Error.WriteLine($"[Callback] {line}");
}