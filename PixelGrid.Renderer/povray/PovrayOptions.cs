using PixelGrid.Renderer.abstracts;

namespace PixelGrid.Renderer.povray;

public class PovrayOptions : Options
{
    public bool AntiAliasing { get; set; } = true;
    public RadiosityOptions Radiosity { get; set; } = new();

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
}
