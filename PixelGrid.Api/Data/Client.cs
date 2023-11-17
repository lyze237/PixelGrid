namespace PixelGrid.Api.Data;

public class Client(string name, string ownerId, bool @public = false)
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Token { get; set; } = Guid.NewGuid().ToString();

    public string Name { get; set; } = name;

    public DateTime? LastConnected { get; set; }
    public DateTime? LastRender { get; set; }

    public bool Public { get; set; } = @public;
    public string OwnerId { get; set; } = ownerId;
    public virtual User Owner { get; set; }
    public virtual List<User> SharedUsers { get; set; } = new();
}