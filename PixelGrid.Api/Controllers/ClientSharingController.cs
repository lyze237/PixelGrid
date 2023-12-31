﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PixelGrid.Api.Data;

namespace PixelGrid.Api.Controllers;

[Authorize]
public class ClientSharingController(ApplicationDbContext dbContext, UserManager<User> userManager) : ShareableResourceController<Client>(dbContext, userManager)
{
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
}