namespace PixelGrid.Server.Domain.Entities;

public class ClientEntity
{
    public long Id { get; set; }
    
    public string Token { get; set; }

    public string Name { get; set; }

    public bool Connected { get; set; }
    public DateTime? LastConnected { get; set; }

    public static ClientEntity CreateClient(string name)
    {
        return new ClientEntity
        {
            Token = Guid.NewGuid().ToString(),
            Name = name
        };
    }
}