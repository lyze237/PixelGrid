using Microsoft.AspNetCore.Authorization;
using PixelGrid.Server.Services;

namespace PixelGrid.Server.Authorization;

public class RendererAuthorizationHandler(RenderClientsManagementService renderClientsManagementService)
    : AuthorizationHandler<RendererAuthorizationRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        RendererAuthorizationRequirement requirement)
    {
        if (await renderClientsManagementService.GetClientFromClaims(context.User) != null)
            context.Succeed(requirement);
    }
}

public class RendererAuthorizationRequirement : IAuthorizationRequirement
{
}