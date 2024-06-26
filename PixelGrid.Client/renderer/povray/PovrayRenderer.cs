using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using PixelGrid.Client.renderer.abstracts;
using PixelGrid.Client.renderer.Extensions;

namespace PixelGrid.Client.renderer.povray;

public partial class PovrayRenderer : IRenderer
{
    public string ExePath { get; set; }

    public void Render(string workingDirectory, string filename, string outputDirectory, string outputFilename, RenderOptions options, IRenderCallback callback)
    {
        if (options is not PovrayOptions povrayOptions)
            throw new ArgumentException($"The options need to be of type {nameof(PovrayOptions)} in a Povray render",
                nameof(options));
        
        var args = povrayOptions.BuildCommandLineOptions(filename, outputDirectory, outputFilename);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            args.Insert(0, ExePath);
            ExePath = "wsl";
        }

        Console.WriteLine($"Starting: {ExePath} {string.Join(", ", args)}");

        var startInfo = new ProcessStartInfo(ExePath)
        {
            WorkingDirectory = workingDirectory,
            UseShellExecute = false,
            RedirectStandardError = true,
            
        };
        args.ToList().ForEach(arg => startInfo.ArgumentList.Add(arg)); // why

        var process = new Process
        {
            StartInfo = startInfo
        };
        process.ErrorDataReceived += (sender, eventArgs) => ParsePovrayOutput(eventArgs.Data, callback);

        process.Start();
        process.BeginErrorReadLine();

        process.WaitForExit();
        callback.OnCompleted(process.ExitCode);
    }

    public RendererCapabilities GetCapabilities() =>
        RendererCapabilities.Frame | RendererCapabilities.Border;

    private void ParsePovrayOutput(string? line, IRenderCallback callback)
    {
        if (line == null)
            return;
        
        callback.OnLog(line);
        
        if (line.Trim().StartsWith("Warning"))
            callback.OnWarning(line);
        
        if (line.Trim().StartsWith("Fatal") && !line.Trim().StartsWith("Fatal Stream to console"))
            callback.OnError(line);

        var match = RenderPattern().Match(line.Trim());
        if (!match.Success)
            return;

        var pixel = match.Groups["pixels"].Value.ToLong();
        var totalPixels = match.Groups["totalPixels"].Value.ToLong();
        
        callback.OnProgress(new PovrayCallbackStatus
        {
            CurrentPixel = pixel,
            TotalPixels = totalPixels
        });
    }

    [GeneratedRegex(@"Rendered (?<pixels>\d+) of (?<totalPixels>\d+) pixels")]
    private static partial Regex RenderPattern();
}