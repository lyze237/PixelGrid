using System.Diagnostics;
using System.Text.RegularExpressions;
using PixelGrid.Renderer.abstracts;
using PixelGrid.Renderer.Extensions;

namespace PixelGrid.Renderer.blender;

public partial class BlenderRenderer : IRenderer
{
    public void Render(string workingDirectory, string filename, string outputDirectory, string outputFilename, Options options,
        IRenderCallback callback)
    {
        if (options is not BlenderOptions blenderOptions)
            throw new ArgumentException($"The options need to be of type {nameof(BlenderOptions)} in a Blender render",
                nameof(options));

        const string exe = @"C:\Users\lyze\AppData\Roaming\BlenderInstalls\stable\blender-3.6.12-lts.626a6b1c6799\blender.exe";

        var args = blenderOptions.BuildCommandLineOptions(filename, outputDirectory, outputFilename);
        Console.WriteLine($"Starting: {exe} {string.Join(", ", args)}");

        var startInfo = new ProcessStartInfo(exe)
        {
            WorkingDirectory = workingDirectory,
            UseShellExecute = false,
            RedirectStandardOutput = true,
        };
        args.ToList().ForEach(arg => startInfo.ArgumentList.Add(arg)); // why

        var process = new Process
        {
            StartInfo = startInfo
        };
        process.OutputDataReceived += (sender, eventArgs) => ParseBlenderOutput(eventArgs.Data, callback);

        process.Start();
        process.BeginOutputReadLine();

        process.WaitForExit();
        callback.OnCompleted(process.ExitCode);
    }

    public RendererCapabilities GetCapabilities() =>
        RendererCapabilities.Frame | RendererCapabilities.Animation | RendererCapabilities.Border;

    private static void ParseBlenderOutput(string? line, IRenderCallback callback)
    {
        if (line == null)
            return;
        
        callback.OnLog(line);

        if (line.Trim().StartsWith("WARN"))
            callback.OnWarning(line);

        if (line.Trim().StartsWith("EXCEPTION"))
            callback.OnError(line);

        var isInitializing = line.Split("|").Contains("Initializing");
        
        if (isInitializing)
            callback.OnStart();
        
        var match = FramePattern().Match(line.Trim());

        if (!match.Success) 
            return;
        
        var frame = match.Groups["frame"].Value.ToInt();
        var sample = match.Groups["sample"].Value.ToInt();
        var totalSample = match.Groups["totalSample"].Value.ToInt();

        var memory = match.Groups["memory"].Value.ToFloat();
        var peakMemory = match.Groups["peak"].Value.ToFloat();

        var remaining = match.Groups["remaining"].Value;

        callback.OnProgress(new BlenderCallbackStatus
        {
            Frame = frame,
            CurrentSample = sample,
            TotalSamples = totalSample,
            MemoryUsage = memory,
            PeakMemoryUsage = peakMemory,
            RemainingTime = remaining
        });
    }


    [GeneratedRegex(@"Fra:(?<frame>\d) Mem:(?<memory>\d+(\.\d+)?)M \(Peak (?<peak>\d+(\.\d+)?)M\) \|.*Remaining:(?<remaining>[\d:.]+) \|.*Sample (?<sample>\d+)\/(?<totalSample>\d+)")]
    private static partial Regex FramePattern();
}