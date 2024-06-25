using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using PixelGrid.Server.Db;

namespace PixelGrid.Server.Controllers;

public class ClientController(ApplicationDbContext dbContext, ILogger<ClientController> logger) : Client.ClientBase
{
    [Authorize]
    public override async Task<ClientRegisterResponse> Register(ClientRegisterRequest request, ServerCallContext context)
    {
        logger.LogInformation("Registering client {name}", request.Name);

        var client = dbContext.Clients.Add(Db.Client.CreateClient(request.Name));
        await dbContext.SaveChangesAsync();

        return new ClientRegisterResponse { 
            Success = true,
            Token = client.Entity.Token
        };
    }
}