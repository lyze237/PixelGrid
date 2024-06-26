using PixelGrid.Server.Domain.Entities;

namespace PixelGrid.Server.Services.Jwt;

public interface IJwtService
{
    Task<string> GenerateTokenAsync(User user);
}