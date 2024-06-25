using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PixelGrid.Server.Options;

namespace PixelGrid.Server.Services.Jwt;

public class JwtService : IJwtService
{
    private readonly IOptions<JwtOptions> options;

    public JwtService(IOptions<JwtOptions> options) => 
        this.options = options;

    public Task<string> GenerateTokenAsync()
    {
        var securityToken =new JwtSecurityTokenHandler().CreateToken(new SecurityTokenDescriptor
        {
            Issuer = options.Value.Issuer,
            Audience = options.Value.Audience,
            Expires = DateTime.UtcNow.AddHours(1), // expires after 1 hour
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.Key)), SecurityAlgorithms.HmacSha256Signature)
        });

        return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(securityToken));
    }
}