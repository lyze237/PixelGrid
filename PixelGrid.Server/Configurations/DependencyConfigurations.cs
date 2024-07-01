using Microsoft.AspNetCore.Authorization;
using PixelGrid.Server.Authorization;
using PixelGrid.Server.Domain.Entities;
using PixelGrid.Server.Domain.Repositories;
using PixelGrid.Server.Infra.Repositories;
using PixelGrid.Server.Services;

namespace PixelGrid.Server.Configurations;

/// <summary>
/// This static class is responsible for registering the dependencies used in the application.
/// </summary>
public static class DependencyConfigurations
{
    /// <summary>
    /// Registers the dependencies used in the application.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static void RegisterDependencies(this IServiceCollection services)
    {
        services.AddScoped<IAuthorizationHandler, RendererAuthorizationHandler>();
        
        services.AddScoped<FilesService>();
        services.AddScoped<UserManagementService>();
        services.AddScoped<RenderClientsManagementService>();
        services.AddScoped<RenderJobManagementService>();
        services.AddScoped<JwtService>();
        
        services.AddScoped<IUserManagementRepository, DbUserManagementRepository>();
        services.AddScoped<IProjectRepository, DbProjectRepository>();
        services.AddScoped<IRenderClientRepository, DbRenderClientRepository>();
        services.AddScoped<IRenderClientProgramVersionRepository, DbRenderClientProgramVersionRepository>();
    }
}