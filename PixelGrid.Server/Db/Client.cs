namespace PixelGrid.Server.Db;

public class Client
{
    public long Id { get; set; }
    
    public string Token { get; set; }

    public string Name { get; set; }

    public bool Connected { get; set; }
    public DateTime? LastConnected { get; set; }

    public static Client CreateClient(string name)
    {
        return new Client
        {
            Token = Guid.NewGuid().ToString(),
            Name = name
        };
    }
}