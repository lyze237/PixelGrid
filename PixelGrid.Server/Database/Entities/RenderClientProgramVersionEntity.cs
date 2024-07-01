using PixelGrid.Shared.Renderer;

namespace PixelGrid.Server.Database.Entities;

public class RenderClientProgramVersionEntity
{
    public long Id { get; set; }
    public string Version { get; set; }
    public RendererCapabilities? RendererCapabilities { get; set; }
    public RenderClientEntity RenderClient { get; set; }
    public RenderType Type { get; set; }
}