using System.Security.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using PixelGrid.Server.Domain.Entities;
using PixelGrid.Server.Domain.Repositories;
using PixelGrid.Server.Hubs;
using PixelGrid.Shared.Hubs;

namespace PixelGrid.Server.Services;

/// <summary>
/// Provides methods for managing the connection status of clients and handling authentication in the rendering service.
/// </summary>
public class RenderClientsManagementService(
    IClientRepository clientRepository,
    IHubContext<RenderHub, IRenderHub.IClient> renderHub)
{
    /// <summary>
    /// Sets the connection status of a client and handles authentication in the rendering service.
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal representing the authenticated user.</param>
    /// <param name="connected">A boolean value indicating whether the client is connected or not.</param>
    /// <exception cref="AuthenticationException">Thrown when the authentication token is null or the client is not authenticated.</exception>
    public async Task SetClientConnectionStatus(ClaimsPrincipal? claimsPrincipal, bool connected)
    {
        var client = await GetClientFromClaims(claimsPrincipal);
        if (client == null)
            throw new AuthenticationException();
        
        if (connected)
            clientRepository.SetConnected(client);
        else
            clientRepository.SetDisconnected(client);
        await clientRepository.SaveAsync();
    }

    public async Task<RenderClientEntity?> GetClientFromClaims(ClaimsPrincipal? claimsPrincipal)
    {
        var token = GetAuthenticationToken(claimsPrincipal);
        if (token == null)
            return null;

        return await clientRepository.GetByTokenAsync(token) ?? null;
    }

    /// <summary>
    /// Retrieves the authentication token for the given claims principal.
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal representing the authenticated user.</param>
    /// <returns>The authentication token, or null if not found.</returns>
    private static string? GetAuthenticationToken(ClaimsPrincipal? claimsPrincipal) =>
        claimsPrincipal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value ?? null;
}