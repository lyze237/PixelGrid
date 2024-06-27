using PixelGrid.Server.Domain;
using PixelGrid.Server.Domain.Entities;

namespace PixelGrid.Server.Services;

/// ClientService class is responsible for managing client operations.
/// /
public class ClientService(JwtService jwtService, ApplicationDbContext dbContext, ILogger<ClientService> logger)
{
    /// <summary>
    /// Registers a client and returns the registration response.
    /// </summary>
    /// <param name="request">The registration request.</param>
    /// <returns>The registration response.</returns>
    public async Task<RenderClientRegisterResponse> Register(RenderClientRegisterRequest request)
    {
        logger.LogInformation("Registering client {Name}", request.Name);

        var client = dbContext.RenderClients.Add(RenderClientEntity.CreateClient(request.Name));
        await dbContext.SaveChangesAsync();

        return new RenderClientRegisterResponse { 
            Success = true,
            Token = await jwtService.GenerateClientTokenAsync(client.Entity)
        };
    }
}