using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PixelGrid.Server.Db;

public class ApplicationDbContext (DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<User, Role, string>(options)
{
    public DbSet<Client> Clients { get; set; }
}