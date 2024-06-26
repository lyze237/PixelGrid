using PixelGrid.Server.Domain;
using PixelGrid.Server.Domain.Entities;

namespace PixelGrid.Server.Services;

public class ClientService(JwtService jwtService, ApplicationDbContext dbContext, ILogger<ClientService> logger)
{
    public async Task<ClientRegisterResponse> Register(ClientRegisterRequest request)
    {
        logger.LogInformation("Registering client {name}", request.Name);

        var client = dbContext.Clients.Add(ClientEntity.CreateClient(request.Name));
        await dbContext.SaveChangesAsync();

        return new ClientRegisterResponse { 
            Success = true,
            Token = await jwtService.GenerateClientTokenAsync(client.Entity)
        };
    }
}