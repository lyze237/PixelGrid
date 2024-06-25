namespace PixelGrid.Server.Options;

public class JwtOptions
{
    public string Authority { get; set; }
    public string Audience { get; set; }
    public string Issuer { get; set; }
    public string Key { get; set; }
}