using PixelGrid.Renderer;
using PixelGrid.Renderer.blender;
using PixelGrid.Renderer.Callbacks;
using PixelGrid.Renderer.povray;

namespace PixelGrid.Client;

public class OfflineWorker(ILogger<OfflineWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        const string folder = @"C:\Users\lyze\Downloads\povfile";
        const string file = "Ansichten_2.pov";

        var outputFolder = "export";

        const string outputFile = "Output.png";
        
        var manager = new RenderManager();
        manager.Render("povray", folder, file, outputFolder, outputFile, new PovrayOptions
        {
            
        }, new StdoutCallback());
    }
}