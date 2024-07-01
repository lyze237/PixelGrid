namespace PixelGrid.Server.Domain.Entities;

public class WorkspaceEntity
{
    public long Id { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }
    
    public TeamEntity Team { get; set; }
}