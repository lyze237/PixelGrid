using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PixelGrid.Server.Database;
using PixelGrid.Server.Options;

namespace PixelGrid.Server.Configurations;

public static class Configurations
{
    public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
                               throw new InvalidOperationException("Connection string 'DefaultConnection' not set");
        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString));
    }

    public static void AddSwaggerThings(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(config =>
        {
            config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Enter token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "bearer"
            });

            config.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }

    public static void AddSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        services.Configure<RendererOptions>(configuration.GetSection("Renderer"));
    }
}