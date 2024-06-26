using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using PixelGrid.Renderer.abstracts;
using PixelGrid.Renderer.Extensions;

namespace PixelGrid.Renderer.povray;

public partial class PovrayRenderer : IRenderer
{
    public void Render(string workingDirectory, string filename, string outputDirectory, string outputFilename, Options options, IRenderCallback callback)
    {
        if (options is not PovrayOptions povrayOptions)
            throw new ArgumentException($"The options need to be of type {nameof(PovrayOptions)} in a Povray render",
                nameof(options));
        
        var exe = "povray";
        var args = povrayOptions.BuildCommandLineOptions(filename, outputDirectory, outputFilename);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            exe = "wsl";
            args.Insert(0, "povray");
        }

        Console.WriteLine($"Starting: {exe} {string.Join(", ", args)}");

        var startInfo = new ProcessStartInfo(exe)
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