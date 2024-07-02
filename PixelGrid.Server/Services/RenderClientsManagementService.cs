using System.Security.Authentication;
using System.Security.Claims;
using FluentResults;
using Microsoft.AspNetCore.SignalR;
using PixelGrid.Server.Database.Entities;
using PixelGrid.Server.Database.Repositories;
using PixelGrid.Server.Infra.Exceptions;
using PixelGrid.Shared.Models.Controller;
using PixelGrid.Shared.Renderer;

namespace PixelGrid.Server.Services;

/// <summary>
/// Provides methods for managing the connection status of clients and handling authentication in the rendering service.
/// </summary>
public class RenderClientsManagementService(
    RenderClientRepository renderClientRepository,
    RenderClientProgramVersionRepository programVersionRepository,
    JwtService jwtService,
    ILogger<RenderClientsManagementService> logger)
{
    /// <summary>
    /// Registers a client and returns the registration response.
    /// </summary>
    /// <param name="request">The registration request.</param>
    /// <returns>The registration response.</returns>
    public async Task<Result<RenderClientRegisterResponse>> Register(RenderClientRegisterRequest request)
    {
        logger.LogInformation("Registering client {Name}", request.Name);

        var client = await renderClientRepository.CreateAsync(RenderClientEntity.CreateClient(request.Name));
        await renderClientRepository.SaveAsync();

        var token = await jwtService.GenerateClientTokenAsync(client);
        return Result.Ok(new RenderClientRegisterResponse(token));
    }

    /// <summary>
    /// Sets the connection status of a client and handles authentication in the rendering service.
    /// </summary>
    /// <exception cref="AuthenticationException">Thrown when the authentication token is null or the client is not authenticated.</exception>
    public async Task ClientRegistered(HubCallerContext context)
    {
        var client = await GetClientFromClaims(context.User);
        if (client == null)
            throw new AuthenticationException();

        await programVersionRepository.RemoveAllFromClientAsync(client);
        renderClientRepository.SetConnected(client, context.ConnectionId);
        await renderClientRepository.SaveAsync();
    }
    
    public async Task ClientDisconnected(HubCallerContext context)
    {
        var client = await GetClientFromClaims(context.User);
        if (client == null)
            throw new AuthenticationException();

        renderClientRepository.SetDisconnected(client);
        await renderClientRepository.SaveAsync();
    } 


    public async Task<RenderClientEntity?> GetClientFromClaims(ClaimsPrincipal? claimsPrincipal)
    {
        var token = GetAuthenticationToken(claimsPrincipal);
        if (token == null)
            return null;

        return await renderClientRepository.GetByTokenAsync(token) ?? null;
    }
    
    public async Task RegisterProgram(HubCallerContext context, RenderType type, string version, RendererCapabilities rendererCapabilities)
    {
        var client = await GetClientFromClaims(context.User);
        if (client == null)
            throw new EntityNotFoundException<RenderClientEntity>("Couldn't find render client");
        
        await programVersionRepository.CreateAsync(new RenderClientProgramVersionEntity
        {
            RendererCapabilities = rendererCapabilities,
            Type = type,
            Version = version,
            RenderClient = client
        });
        await programVersionRepository.SaveAsync();
    }

    /// <summary>
    /// Retrieves the authentication token for the given claims principal.
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal representing the authenticated user.</param>
    /// <returns>The authentication token, or null if not found.</returns>
    private static string? GetAuthenticationToken(ClaimsPrincipal? claimsPrincipal) =>
        claimsPrincipal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value ?? null;
}