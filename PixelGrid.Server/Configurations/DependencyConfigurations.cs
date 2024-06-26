using PixelGrid.Server.Domain.Repositories;
using PixelGrid.Server.Infra.Repositories;
using PixelGrid.Server.Services;

namespace PixelGrid.Server.Configurations;

public static class DependencyConfigurations
{
    public static void RegisterDependencies(this IServiceCollection services)
    {
        services.AddScoped<UserManagementService>();
        services.AddScoped<ClientService>();
        services.AddScoped<JwtService>();
        
        services.AddScoped<IUserManagementRepository, DbUserManagementRepository>();
        services.AddScoped<IClientRepository, DbClientRepository>();
    }
}