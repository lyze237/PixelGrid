namespace PixelGrid.Api.Data;

public class Job
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public BlendFile? BlendFile { get; set; }
    public Project Project { get; set; }

    public User Owner { get; set; }
    public List<Client> Clients { get; set; }

    public DateTime? StartedAt { get; set; }

    public int? Frame { get; set; }
    
    public bool Animation { get; set; }
    public int FrameStart { get; set; }
    public int FrameEnd { get; set; }
}