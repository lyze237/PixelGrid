using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using PixelGrid.Api.Data;

namespace PixelGrid.Api.Controllers;

[Authorize]
public class ProjectSharingController(ApplicationDbContext dbContext, UserManager<User> userManager) : ShareableResourceController<Project>(dbContext, userManager)
{
    
}