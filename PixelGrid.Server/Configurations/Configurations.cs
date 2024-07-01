using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PixelGrid.Server.Database;
using PixelGrid.Server.Options;

namespace PixelGrid.Server.Configurations;

/// <summary>
/// Provides extension methods for configuring various options in the application.
/// </summary>
public static class Configurations
{
    /// <summary>
    /// Adds the database configuration to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <exception cref="InvalidOperationException">Thrown if the connection string 'DefaultConnection' is not set.</exception>
    public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
                               throw new InvalidOperationException("Connection string 'DefaultConnection' not set");
        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString));
    }

    /// <summary>
    /// Adds Swagger and gRPC documentation configuration to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    public static void AddSwaggerWithGrpc(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpcSwagger();
        services.AddSwaggerGen(config =>
        {
            var filePath = Path.Combine(AppContext.BaseDirectory, "PixelGrid.Server.xml");
            config.IncludeXmlComments(filePath);
            config.IncludeGrpcXmlComments(filePath, includeControllerXmlComments: true);

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

    /// <summary>
    /// Adds the settings configuration to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    public static void AddSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        services.Configure<RendererOptions>(configuration.GetSection("Renderer"));
    }
}