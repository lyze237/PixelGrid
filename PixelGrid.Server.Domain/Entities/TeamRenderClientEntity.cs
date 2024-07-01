namespace PixelGrid.Server.Domain.Entities;

public class TeamRenderClientEntity
{
    public long Id { get; set; }

    public TeamEntity Team { get; set; }
    public RenderClientEntity RenderClient { get; set; }
}