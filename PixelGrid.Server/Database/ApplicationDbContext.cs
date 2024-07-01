using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PixelGrid.Server.Database.Entities;

namespace PixelGrid.Server.Database;

public class ApplicationDbContext (DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<UserEntity, RoleEntity, string>(options)
{
    public DbSet<RenderClientEntity> RenderClients { get; set; }
    public DbSet<TeamEntity> Teams { get; set; }
    public DbSet<TeamMemberEntity> TeamMembers { get; set; }
    public DbSet<TeamRenderClientEntity> TeamRenderClients { get; set; }
    public DbSet<WorkspaceEntity> Workspaces { get; set; }
    public DbSet<ProjectEntity> Projects { get; set; }
    public DbSet<ProjectResultEntity> ProjectResults { get; set; }
}