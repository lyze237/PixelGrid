using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PixelGrid.Server.Domain.Entities;

namespace PixelGrid.Server.Domain;

public class ApplicationDbContext (DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<User, Role, string>(options)
{
    public DbSet<Client> Clients { get; set; }
}