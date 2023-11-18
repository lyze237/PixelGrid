using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PixelGrid.Api.Data;

namespace PixelGrid.Api.Controllers;

public abstract class ShareableResourceController<T>(ApplicationDbContext dbContext, UserManager<User> userManager) : Controller where T : ShareableResource
{
    public async Task<IActionResult> Index(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest("No id given.");
        
        var user = await userManager.GetUserAsync(User) ?? throw new ArgumentException("User is null?");
        var resource = await dbContext.Set<T>()
            .Include(r => r.SharedWith)
            .FirstOrDefaultAsync(c => c.Id == id && c.Owner == user);
        
        if (resource == null)
            return BadRequest("Id not found or not owner.");

        var usersSharedWidth = await dbContext.Users.Where(u => resource.SharedWith.Contains(u)).ToListAsync();
        var allUsers = await dbContext.Users.Where(u => u != user).ToListAsync();

        allUsers.RemoveAll(u => usersSharedWidth.Contains(u));
        
        return View(new ShareableResourceIndexModel(resource, allUsers));
    }
    
    public async Task<IActionResult> Share(string resourceId, string userId)
    {
        if (string.IsNullOrWhiteSpace(resourceId) || string.IsNullOrWhiteSpace(userId))
            return BadRequest($"No {nameof(resourceId)} or {nameof(userId)} given.");
        
        var user = await userManager.GetUserAsync(User) ?? throw new ArgumentException("User is null?");
        var resource = await dbContext.Set<T>()
            .Include(r => r.SharedWith)
            .FirstOrDefaultAsync(r => r.Id == resourceId && r.Owner == user);
        
        var sharedUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        
        if (resource == null || sharedUser == null)
            return BadRequest("Id not found or not owner.");
        
        resource.SharedWith.Add(sharedUser);
        await dbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Index), routeValues: new { id = resourceId });
    }
    
    public async Task<IActionResult> Delete(string resourceId, string userId)
    {
        if (string.IsNullOrWhiteSpace(resourceId) || string.IsNullOrWhiteSpace(userId))
            return BadRequest($"No {nameof(resourceId)} or {nameof(userId)} given.");
        
        var user = await userManager.GetUserAsync(User) ?? throw new ArgumentException("User is null?");
        var resource = await dbContext.Set<T>()
            .Include(r => r.SharedWith)
            .FirstOrDefaultAsync(r => r.Id == resourceId && r.Owner == user);
        
        var sharedUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        
        if (resource == null || sharedUser == null)
            return BadRequest("Id not found or not owner.");

        resource.SharedWith.Remove(sharedUser);
        await dbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Index), routeValues: new { id = resourceId });
    }
    
    public record ShareableResourceIndexModel(T Resource, List<User> Users);
}