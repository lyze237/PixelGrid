using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PixelGrid.Api.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<User, Role, string>(options)
{
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<BlendFile> BlendFiles => Set<BlendFile>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<File> Files => Set<File>();
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Client>()
            .HasOne<User>(c => c.Owner)
            .WithMany(u => u.OwnedClients)
            .HasForeignKey(c => c.OwnerId)
            .IsRequired();

        builder.Entity<Client>()
            .HasMany<User>(c => c.SharedWith)
            .WithMany(u => u.SharedClients);
        
        builder.Entity<Project>()
            .HasOne<User>(c => c.Owner)
            .WithMany(u => u.OwnedProjects)
            .HasForeignKey(c => c.OwnerId)
            .IsRequired();

        builder.Entity<Project>()
            .HasMany<User>(c => c.SharedWith)
            .WithMany(u => u.SharedProjects);
    }
}