using System.Runtime.Serialization;
using PixelGrid.Renderer.abstracts;

namespace PixelGrid.Renderer.blender;

public class BlenderOptions(Engine engine) : Options
{
    public Engine Engine { get; private set; } = engine;
    public int Threads { get; set; } = 0; // 0 for processor count
    public BlenderRenderFormat RenderFormat { get; set; } = BlenderRenderFormat.Png;
    public string? Scene { get; set; } = null;

    public string[] BuildCommandLineOptions(string filename, string outputDirectory)
    {
        var args = new List<string>
        {
            "--factory-startup",
            "-noaudio",
            
            "-b",
            filename,
            "-o",
            outputDirectory,

            "-E",
            Engine.GetEnumValue() ?? throw new ArgumentException("Enum does not have an EnumMember Attribute",
                nameof(Engine)),

            "-t",
            Threads.ToString(),

            "-F",
            RenderFormat.GetEnumValue() ??
            throw new ArgumentException("Enum does not have an EnumMember Attribute", nameof(RenderFormat)),
        };

        if (Scene != null)
            args.AddRange(new[] {"-S", Scene});

        if (Animation != null)
        {
            args.AddRange(new [] { "-s", Animation.StartFrame.ToString(), "-e", Animation.EndFrame.ToString(), "-a" });
        }
        else
        {
            args.Add("-f");
            if (CustomFrame.HasValue)
                args.Add(CustomFrame.Value.ToString());
        }

        if (this is CyclesBlenderOptions cyclesOptions)
        {
            args.AddRange(new[]
            {
                "--",
                "--cycles-device",
                cyclesOptions.Device.GetEnumValue() ??
                throw new ArgumentException("Enum does not have an EnumMember Attribute", nameof(cyclesOptions.Device)),
            });
        }

        return args.ToArray();
    }
}

public class CyclesBlenderOptions() : BlenderOptions(Engine.Cycles)
{
    public CyclesDevice Device { get; set; } = CyclesDevice.Cpu;
}

public enum BlenderRenderFormat
{
    [EnumMember(Value = "TGA")] Tga,
    [EnumMember(Value = "RAWTGA")] RawTga,
    [EnumMember(Value = "JPEG")] Jpeg,
    [EnumMember(Value = "IRIS")] Iris,
    [EnumMember(Value = "AVIRAW")] AviRaw,
    [EnumMember(Value = "AVIJPEG")] AviJpeg,
    [EnumMember(Value = "PNG")] Png,
    [EnumMember(Value = "BMP")] Bmp,
    [EnumMember(Value = "HDR")] Hdr,
    [EnumMember(Value = "TIFF")] Tiff
}

public enum CyclesDevice
{
    [EnumMember(Value = "CPU")] Cpu = 1,
    [EnumMember(Value = "CUDA")] Cuda = 2,
    [EnumMember(Value = "CUDA+CPU")] CudaCpu = 3,
    [EnumMember(Value = "OPTIX")] Optix = 4,
    [EnumMember(Value = "OPTIX+CPU")] OptixCpu = 5,
    [EnumMember(Value = "HIP")] Hip = 6,
    [EnumMember(Value = "HIP+CPU")] HipCpu = 7,
    [EnumMember(Value = "ONEAPI")] OneApi = 8,
    [EnumMember(Value = "ONEAPI+CPU")] OneApiCpu = 9,
    [EnumMember(Value = "METAL")] Metal = 10,
    [EnumMember(Value = "METAL+CPU")] MetalCpu = 11
}

public enum Engine
{
    [EnumMember(Value = "CYCLES")] Cycles = 1,
    [EnumMember(Value = "BLENDER_EEVEE")] BlenderEevee = 2,

    [EnumMember(Value = "BLENDER_WORKBENCH")]
    BlenderWorkbench = 3
}

public static class EnumExtensions
{
    public static string? GetEnumValue(this Enum engine)
    {
        var member = engine.GetType().GetMember(engine.ToString());
        var attribute = member.FirstOrDefault()?.GetCustomAttributes(typeof(EnumMemberAttribute), false);
        return attribute?.Cast<EnumMemberAttribute>().FirstOrDefault()?.Value;
    }
}