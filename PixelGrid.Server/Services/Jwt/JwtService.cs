using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PixelGrid.Server.Domain.Entities;
using PixelGrid.Server.Options;

namespace PixelGrid.Server.Services.Jwt;

public class JwtService(UserManager<User> userManager, IOptions<JwtOptions> options) : IJwtService
{
    public async Task<string> GenerateTokenAsync(User user)
    {
        var roles = await userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName),
        };

        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));
        
        var securityToken = new JwtSecurityTokenHandler().CreateToken(new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = options.Value.Issuer,
            Audience = options.Value.Audience,
            Expires = DateTime.UtcNow.AddHours(1), // expires after 1 hour
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.Key)), SecurityAlgorithms.HmacSha256Signature)
        });

        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }
}