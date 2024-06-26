using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PixelGrid.Server.Domain.Entities;
using PixelGrid.Server.Options;

namespace PixelGrid.Server.Services;

public class JwtService(UserManager<UserEntity> userManager, IOptions<JwtOptions> options)
{
    public async Task<string> GenerateUserTokenAsync(UserEntity userEntity)
    {
        var roles = await userManager.GetRolesAsync(userEntity);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userEntity.Id),
            new(ClaimTypes.Name, userEntity.UserName),
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
    
    public async Task<string> GenerateClientTokenAsync(ClientEntity client)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, client.Id.ToString()),
            new(ClaimTypes.Actor, client.Token),
        };

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