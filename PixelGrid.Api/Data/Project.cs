namespace PixelGrid.Api.Data;

public class Project(string name, string ownerId) : ShareableResource(ownerId)
{
    public string Name { get; set; } = name;

    public virtual List<BlendFile> BlendFiles { get; set; } = new();
    public virtual List<File> Files { get; set; } = new();
}