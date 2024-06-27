namespace PixelGrid.Server.Domain.Entities;

public class RenderClientEntity
{
    public long Id { get; set; }
    
    public string Token { get; set; }

    public string Name { get; set; }

    public bool Connected { get; set; }
    public DateTime? LastConnected { get; set; }

    public static RenderClientEntity CreateClient(string name)
    {
        return new RenderClientEntity
        {
            Token = Guid.NewGuid().ToString(),
            Name = name
        };
    }
}