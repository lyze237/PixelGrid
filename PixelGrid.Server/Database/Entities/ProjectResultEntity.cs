namespace PixelGrid.Server.Database.Entities;

public class ProjectResultEntity
{
    public long Id { get; set; }

    public ProjectEntity Project { get; set; }

    public string OutputPath { get; set; }
    
    public int Frame { get; set; }
    public int FrameProgress { get; set; }
}