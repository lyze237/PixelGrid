using PixelGrid.Server.Domain.Repositories;

namespace PixelGrid.Server.Services;

/// <summary>
/// Provides user management functionality such as registering and logging in users.
/// </summary>
public class UserManagementService(
    JwtService jwtService,
    IUserManagementRepository userManagement,
    ILogger<UserManagementService> logger)
{
    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="request">The registration request containing the user's details.</param>
    /// <returns>A response indicating the success of the registration operation.</returns>
    public async Task<AuthRegisterResponse> Register(AuthRegisterRequest request)
    {
        logger.LogInformation("Creating user {Username}", request.UserName);

        await userManagement.CreateUserAsync(request.UserName, request.Email, request.Password);
        logger.LogInformation("Created user {Username}", request.UserName);

        return new AuthRegisterResponse { Success = true };
    }

    /// <summary>
    /// Logs in a user.
    /// </summary>
    /// <param name="request">The login request containing the user's email and password.</param>
    /// <returns>A response indicating the success of the login operation and the authentication token.</returns>
    public async Task<AuthLoginResponse> Login(AuthLoginRequest request)
    {
        logger.LogInformation("User {Email} tries to login", request.Email);

        var user = await userManagement.CheckUserPasswordAsync(request.Email, request.Password);
        logger.LogInformation("User {Email} logged in successfully", request.Email);

        var token = await jwtService.GenerateUserTokenAsync(user);

        return new AuthLoginResponse
        {
            Success = true,
            Token = token
        };
    }
}