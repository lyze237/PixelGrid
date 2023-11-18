namespace PixelGrid.Api.Data;

public abstract class ShareableResource(string ownerId)
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    public User Owner { get; set; }
    public string OwnerId { get; set; } = ownerId;

    public virtual List<User> SharedWith { get; set; } = new();
}