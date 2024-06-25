namespace PixelGrid.Server.Services.Jwt;

public interface IJwtService
{
    Task<string> GenerateTokenAsync();
}