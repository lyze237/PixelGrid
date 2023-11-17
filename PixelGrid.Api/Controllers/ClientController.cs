using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
using PixelGrid.Api.Data;
using PixelGrid.Api.Options;
using PixelGrid.Api.Views.Client.Models;

namespace PixelGrid.Api.Controllers;

[Authorize]
public class ClientController(ApplicationDbContext dbContext, UserManager<User> userManager,
    IOptions<JwtOptions> jwtOptions) : Controller
{
    private readonly JwtOptions jwtOptions = jwtOptions.Value;

    public async Task<IActionResult> Index()
    {
        var user = await userManager.GetUserAsync(User) ?? throw new ArgumentException("User is null?");

        var model = new ClientIndexModel(
            await dbContext.Clients
                .Include(c => c.SharedUsers)
                .Where(c => c.Owner == user)
                .ToListAsync(),
            await dbContext.Clients
                .Include(c => c.Owner)
                .Where(c => c.SharedUsers.Contains(user))
                .ToListAsync(),
            await dbContext.Clients
                .Include(c => c.Owner)
                .Where(c => c.Public)
                .ToListAsync()
        );
        return View(model);
    }

    public IActionResult Register() =>
        View();

    [HttpPost]
    public async Task<IActionResult> Register(string? name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length < 3)
            return BadRequest("Please enter a valid name.");

        var user = await userManager.GetUserAsync(User) ?? throw new ArgumentException("User is null?");
        var client = new Client(name, user.Id);
        dbContext.Clients.Add(client);
        await dbContext.SaveChangesAsync();

        return View(model: GenerateToken(client));
    }

    public async Task<IActionResult> Delete(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest("No id given.");

        var user = await userManager.GetUserAsync(User) ?? throw new ArgumentException("User is null?");
        var client = await dbContext.Clients
            .Include(c => c.SharedUsers)
            .FirstOrDefaultAsync(c => c.Id == id && c.Owner == user);

        if (client == null)
            return BadRequest("Id not found or not owner.");

        dbContext.Clients.Remove(client);
        await dbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> ToggleVisibility(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest("No id given.");

        var user = await userManager.GetUserAsync(User) ?? throw new ArgumentException("User is null?");
        var client = await dbContext.Clients
            .FirstOrDefaultAsync(c => c.Id == id && c.Owner == user);

        if (client == null)
            return BadRequest("Id not found or not owner.");

        client.Public = !client.Public;
        await dbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private string GenerateToken(Client client)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, client.Id),
            new Claim(JwtRegisteredClaimNames.Name, client.Name),
            new Claim(JwtRegisteredClaimNames.Jti, client.Token),
            new Claim(JwtRegisteredClaimNames.Iss, jwtOptions.Issuer),
            new Claim(JwtRegisteredClaimNames.Aud, jwtOptions.Issuer)
        };

        var token = new JwtSecurityToken(jwtOptions.Issuer,
            jwtOptions.Issuer,
            claims,
            expires: DateTime.Now.AddDays(14),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}