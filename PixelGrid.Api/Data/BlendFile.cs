namespace PixelGrid.Api.Data;

public class BlendFile
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public int Width { get; set; }
    public int Height { get; set; }

    public string[] Scenes { get; set; } = Array.Empty<string>();
    public string[] Cameras { get; set; } = Array.Empty<string>();

    public Project Project { get; set; }
}