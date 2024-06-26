using Microsoft.Extensions.Options;
using PixelGrid.Client.Options;
using PixelGrid.Client.renderer;
using PixelGrid.Client.renderer.abstracts;
using PixelGrid.Client.renderer.blender;
using PixelGrid.Client.renderer.Callbacks;

namespace PixelGrid.Client;

public class OfflineWorker(RenderFactory factory, IOptions<RendererOptions> options, ILogger<OfflineWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        const string folder = @"C:\Users\lyze\Downloads\povfile";
        const string file = "Turnaround.blend";

        var outputFolder = "export";

        const string outputFile = "Output.png";
        Console.WriteLine(options.Value.Versions.Values.Count);

        var renderer = factory.GetRenderer(RenderType.Blender, options.Value.Versions[RenderType.Blender].First(r => r.Version == "3.6.12").Exe);

        for (int i = 0; i < 2; i++)
        {
            renderer.Render(folder, file, outputFolder, $"{i}{outputFile}", new CyclesBlenderOptions()
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