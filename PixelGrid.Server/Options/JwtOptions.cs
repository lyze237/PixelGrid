namespace PixelGrid.Server.Options;

/// <summary>
/// Options for configuring JWT authentication.
/// </summary>
public class JwtOptions
{
    public string Authority { get; set; }
    public string Audience { get; set; }
    public string Issuer { get; set; }
    public string Key { get; set; }
}