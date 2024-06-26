using Grpc.Core;
using PixelGrid.Server.Services;

namespace PixelGrid.Server.Controllers;

public class AuthController(UserManagementService userManagement, ILogger<AuthController> logger) : Auth.AuthBase
{
    public override async Task<AuthRegisterResponse> Register(AuthRegisterRequest request, ServerCallContext context) =>
        await userManagement.Register(request);

    public override async Task<AuthLoginResponse> Login(AuthLoginRequest request, ServerCallContext context)
    {
        return await userManagement.Login(request);
    }
}