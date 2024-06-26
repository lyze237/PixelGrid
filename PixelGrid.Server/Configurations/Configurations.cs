using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PixelGrid.Server.Domain;
using PixelGrid.Server.Domain.Entities;
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
    /// Adds JWT authentication to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="jwtOptions">The JWT options.</param>
    public static void AddJwtAuthentication(this IServiceCollection services, JwtOptions jwtOptions)
    {
        services.AddAuthorizationBuilder();

        services.AddIdentityCore<UserEntity>()
            .AddRoles<RoleEntity>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        services.Configure<IdentityOptions>(options => options.User.RequireUniqueEmail = true);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.Authority = jwtOptions.Authority;
            options.Audience = jwtOptions.Audience;
            options.RequireHttpsMetadata = false;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key))
            };
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Headers.Authorization;

                    var path = context.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                        context.Token = accessToken.ToString().Replace("Bearer ", "");

                    return Task.CompletedTask;
                },

                OnAuthenticationFailed = context =>
                {
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        context.Response.Headers.Append("Token-Expired", "true");

                    return Task.CompletedTask;
                }
            };
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
    }
}