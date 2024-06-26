using PixelGrid.Renderer;
using PixelGrid.Renderer.abstracts;
using PixelGrid.Renderer.blender;
using PixelGrid.Renderer.Callbacks;
using PixelGrid.Renderer.povray;

namespace PixelGrid.Client;

public class OfflineWorker(ILogger<OfflineWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        const string folder = @"C:\Users\lyze\Downloads\povfile";
        const string file = "Turnaround.blend";

        var outputFolder = "export";

        const string outputFile = "Output.png";

        var manager = new RenderManager();
        for (int i = 0; i < 2; i++)
        {
            manager.Render("blender", folder, file, outputFolder, $"{i}{outputFile}", new CyclesBlenderOptions()
            {
                Width = 300,
                Height = 400,
                Device = CyclesDevice.Cpu,
                Border = i == 0 ?
                    new BorderOptions(0, 0, 1, 0.5f) : 
                    new BorderOptions(0, 0.5f, 1f, 1f)
            }, new StdoutCallback());
        }

        /*
        manager.Render("povray", folder, file, outputFolder, outputFile,
            new PovrayOptions(),
            new StdoutCallback());
        */
    }
}