using System.Runtime.Serialization;
using PixelGrid.Shared.Renderer.Exceptions;

namespace PixelGrid.Shared.Renderer.Options;

public class BlenderOptions(Engine engine) : RenderOptions
{
    public Engine Engine { get; private set; } = engine;
    public int Threads { get; set; } = 0; // 0 for processor count
    public BlenderRenderFormat RenderFormat { get; set; } = BlenderRenderFormat.Png;
    public string? Scene { get; set; } = null;

    public virtual List<string> BuildCommandLineOptions(string filename, string outputDirectory, string outputFilename)
    {
        var args = new List<string>
        {
            "--factory-startup",
            "-noaudio",

            "-b",
            filename,
            "-o",
            $"{(Path.IsPathRooted(outputDirectory) ? "" : "//")}{outputDirectory}/{Path.GetFileNameWithoutExtension(outputFilename)}",

            "-E",
            Engine.GetEnumValue() ?? throw new OptionsException("Unknown enum", nameof(Engine)),

            "-t",
            Threads.ToString(),

            "-F",
            RenderFormat.GetEnumValue() ?? throw new OptionsException("Unknown", nameof(RenderFormat))
        };

        if (Scene != null)
            args.AddRange(new[] { "-S", Scene });

        if (Border != null)
        {
            args.Add("--python-expr");
            args.Add(string.Join("\n", new List<string>
            {
                "import bpy",
                "for scene in bpy.data.scenes:",
                "    scene.render.use_border = True",
                $"    scene.render.border_min_x = {Border.BorderMinX}",
                $"    scene.render.border_min_y = {Border.BorderMinY}",
                $"    scene.render.border_max_x = {Border.BorderMaxX}",
                $"    scene.render.border_max_y = {Border.BorderMaxY}"
            }));
        }

        args.AddRange(Animation != null
            ? new[] { "-s", Animation.StartFrame.ToString(), "-e", Animation.EndFrame.ToString(), "-a" }
            : ["-f", Frame.ToString()]);

        return args;
    }
}

public class CyclesBlenderOptions() : BlenderOptions(Engine.Cycles)
{
    public CyclesDevice Device { get; set; } = CyclesDevice.Cpu;

    public override List<string> BuildCommandLineOptions(string filename, string outputDirectory, string outputFilename)
    {
        var args = base.BuildCommandLineOptions(filename, outputDirectory, outputFilename);

        args.AddRange(new[]
        {
            "--",
            "--cycles-device",
            Device.GetEnumValue() ??
            throw new OptionsException("Unknown enum", nameof(Device))
        });

        return args;
    }
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