using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PixelGrid.Server.Domain.Entities;

namespace PixelGrid.Server.Domain;

public class ApplicationDbContext (DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<UserEntity, RoleEntity, string>(options)
{
    public DbSet<RenderClientEntity> RenderClients { get; set; }
}