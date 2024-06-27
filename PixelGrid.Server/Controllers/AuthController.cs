using Grpc.Core;
using PixelGrid.Server.Services;

namespace PixelGrid.Server.Controllers;

/// <summary>
/// Class responsible for user authentication and authorization.
/// </summary>
public class AuthController(UserManagementService userManagement, ILogger<AuthController> logger) : AuthControllerProto.AuthControllerProtoBase
{
    /// <summary>
    /// Registers a user.
    /// </summary>
    /// <param name="request">The registration request.</param>
    /// <param name="context">The server call context.</param>
    /// <returns>The registration response.</returns>
    public override async Task<AuthRegisterResponse> Register(AuthRegisterRequest request, ServerCallContext context) =>
        await userManagement.Register(request);

    /// <summary>
    /// Logs in a user.
    /// </summary>
    /// <param name="request">The login request.</param>
    /// <param name="context">The server call context.</param>
    /// <returns>The login response.</returns>
    public override async Task<AuthLoginResponse> Login(AuthLoginRequest request, ServerCallContext context)
    {
        return await userManagement.Login(request);
    }
}