using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PixelGrid.Server.Domain.Entities;
using PixelGrid.Server.Options;

namespace PixelGrid.Server.Services;

/// <summary>
/// Service for generating and managing JSON Web Tokens (JWT).
/// </summary>
public class JwtService(UserManager<UserEntity> userManager, IOptions<JwtOptions> options)
{
    /// <summary>
    /// Generates a JSON Web Token (JWT) for the given user entity.
    /// </summary>
    /// <param name="userEntity">The user entity for which to generate the token.</param>
    /// <returns>The generated JWT token string.</returns>
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
            Expires = DateTime.UtcNow.AddHours(1), // expires after 1 hour
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.Key)), SecurityAlgorithms.HmacSha256Signature)
        });

        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }

    /// <summary>
    /// Generates a JSON Web Token (JWT) for the given client entity.
    /// </summary>
    /// <param name="renderClient">The client entity for which to generate the token.</param>
    /// <returns>The generated JWT token string.</returns>
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
            Expires = DateTime.UtcNow.AddHours(1), // expires after 1 hour
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.Key)), SecurityAlgorithms.HmacSha256Signature)
        });

        return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(securityToken));
    }
}