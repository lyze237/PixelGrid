using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using PixelGrid.Server.Services;

namespace PixelGrid.Server.Controllers;

public class ClientController(ClientService clientService, ILogger<ClientController> logger) : Client.ClientBase
{
    [Authorize]
    public override async Task<ClientRegisterResponse> Register(ClientRegisterRequest request, ServerCallContext context) => 
        await clientService.Register(request);
}