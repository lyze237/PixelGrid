using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PixelGrid.Server.Database.Entities;
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
            new(ClaimTypes.Name, userEntity.UserName!)
        };

        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));
        
        var securityToken = new JwtSecurityTokenHandler().CreateToken(new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = options.Value.Issuer,
            Audience = options.Value.Audience,
            Expires = DateTime.UtcNow.AddDays(1), // expires after 1 day
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.Key)), SecurityAlgorithms.HmacSha256Signature)
        });

        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }

    public Task<string> GenerateClientTokenAsync(RenderClientEntity renderClient)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, renderClient.Id.ToString()),
            new(ClaimTypes.Actor, renderClient.Token)
        };

        var securityToken = new JwtSecurityTokenHandler().CreateToken(new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = options.Value.Issuer,
            Audience = options.Value.Audience,
            Expires = DateTime.UtcNow.AddYears(1), // expires after 1 year
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.Key)), SecurityAlgorithms.HmacSha256Signature)
        });

        return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(securityToken));
    }
}