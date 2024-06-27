using Microsoft.AspNetCore.Authorization;
using PixelGrid.Server.Services;

namespace PixelGrid.Server.Authorization;

/// <summary>
/// Handles the authorization policy for rendering services.
/// </summary>
public class RendererAuthorizationHandler(RenderClientsManagementService renderClientsManagementService)
    : AuthorizationHandler<RendererAuthorizationRequirement>
{
    /// <summary>
    /// Handles the authorization policy for rendering services.
    /// </summary>
    /// <param name="context">The authorization context.</param>
    /// <param name="requirement">The authorization requirement.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        RendererAuthorizationRequirement requirement)
    {
        if (await renderClientsManagementService.GetClientFromClaims(context.User) != null)
            context.Succeed(requirement);
    }
}

/// <summary>
/// Represents the authorization requirement for rendering services.
/// </summary>
public class RendererAuthorizationRequirement : IAuthorizationRequirement
{
}