using System.Globalization;
using System.Runtime.Serialization;

namespace PixelGrid.Shared.Renderer.Options;

public class PovrayOptions : RenderOptions
{
    // https://wiki.povray.org/content/Reference:Tracing_Options#Quality_Settings
    public float? AntiAliasingTreshold { get; set; } = 0.3f;
    public RadiosityOptions Radiosity { get; set; } = new();
    public PovrayRenderFormat RenderFormat { get; set; } = PovrayRenderFormat.Png;
    public int Quality { get; set; } = 9; // 0 - 11
    public int? RenderThreads { get; set; }

    public class RadiosityOptions
    {
        public float PretraceStart { get; set; } = 0.08f;
        public float PretraceEnd { get; set; } = 0.04f;

        public int Count { get; set; } = 300;

        public int NearestCount { get; set; } = 5;
        public float ErrorBound { get; set; }
        public int RecursionLimit { get; set; } = 3;

        public float LowErrorFactor { get; set; } = 0.5f;
        public float GrayTreshold { get; set; } = 0.0f;
        public float MinimumReuse { get; set; } = 0.015f;

        public float Brightness { get; set; } = 1.0f;

        public float AdcBailout { get; set; } = 0.1f;
    }

    public List<string> BuildCommandLineOptions(string filename, string outputDirectory, string outputFilename)
    {
        var args = new List<string>
        {
            filename,
            RenderThreads != null ? $"+WT{RenderThreads}" : "",
            $"+Q{Quality}",
            AntiAliasingTreshold == null
                ? "-A"
                : $"+A{string.Format(CultureInfo.GetCultureInfo("en-us"), "{0:0.#}", AntiAliasingTreshold)}",
            $"+W{Width}",
            $"+H{Height}",
            $"+F{RenderFormat.GetEnumValue()}",
            Border != null ? $"+SC{Border.BorderMinX} +SR{Border.BorderMinY} +EC{Border.BorderMaxX} +ER{Border.BorderMaxY}" : "",
            $"+O{outputDirectory.Replace("\\", "/")}/{outputFilename}",
            "+V",
            "-D"
        };

        return args;
    }
}
public enum PovrayRenderFormat
{
    [EnumMember(Value = "B")] Bitmap,
    [EnumMember(Value = "C")] CTarga24,
    [EnumMember(Value = "E")] OpenExrHdr,
    [EnumMember(Value = "H")] RadianceHdr,
    [EnumMember(Value = "J")] Jpeg,
    [EnumMember(Value = "N")] Png,
    [EnumMember(Value = "P")] Ppm,
    [EnumMember(Value = "T")] UTarga24
}
