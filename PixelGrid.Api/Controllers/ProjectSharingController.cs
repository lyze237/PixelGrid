using Microsoft.AspNetCore.Identity;
using PixelGrid.Api.Data;

namespace PixelGrid.Api.Controllers;

public class ProjectSharingController(ApplicationDbContext dbContext, UserManager<User> userManager) : ShareableResourceController<Project>(dbContext, userManager)
{
    
}