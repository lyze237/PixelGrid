using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PixelGrid.Api.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<User, Role, string>(options)
{
    public DbSet<Client> Clients => Set<Client>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Client>()
            .HasOne<User>(c => c.Owner)
            .WithMany(u => u.OwnedClients)
            .HasForeignKey(c => c.OwnerId)
            .IsRequired();

        builder.Entity<Client>()
            .HasMany<User>(c => c.SharedUsers)
            .WithMany(u => u.SharedClients);
    }
}