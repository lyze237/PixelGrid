using PixelGrid.Renderer;
using PixelGrid.Renderer.blender;
using PixelGrid.Renderer.Callbacks;

namespace PixelGrid.Client;

public class OfflineWorker(ILogger<OfflineWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var manager = new RenderManager();
        manager.Render("blender", @"C:\Users\wml\Desktop\blenderTest", "blender-4.1-splash.blend", @"C:\Users\wml\Desktop\blenderTest", new CyclesBlenderOptions
        {
            Device = CyclesDevice.Cuda
        }, new StdoutCallback());
    }
}