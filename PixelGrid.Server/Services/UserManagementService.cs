using FluentResults;
using PixelGrid.Server.Database.Repositories;
using PixelGrid.Shared.Models.Controller;

namespace PixelGrid.Server.Services;

/// <summary>
/// Provides user management functionality such as registering and logging in users.
/// </summary>
public class UserManagementService(
    JwtService jwtService,
    UserManagementRepository userManagement,
    ILogger<UserManagementService> logger)
{
    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="request">The registration request containing the user's details.</param>
    /// <returns>A response indicating the success of the registration operation.</returns>
    public async Task<Result<AuthRegisterResponse>> Register(AuthRegisterRequest request)
    {
        logger.LogInformation("Creating user {Username}", request.UserName);

        var result = await userManagement.CreateUserAsync(request.UserName, request.Email, request.Password);
        logger.LogInformation("Created user {Username}", request.UserName);

        return result.ToResult(_ => new AuthRegisterResponse());
    }

    /// <summary>
    /// Logs in a user.
    /// </summary>
    /// <param name="request">The login request containing the user's email and password.</param>
    /// <returns>A response indicating the success of the login operation and the authentication token.</returns>
    public async Task<Result<AuthLoginResponse>> Login(AuthLoginRequest request)
    {
        logger.LogInformation("User {Email} tries to login", request.Email);

        var result = await userManagement.CheckUserPasswordAsync(request.Email, request.Password);
        logger.LogInformation("User {Email} logged in successfully", request.Email);

        if (result.IsFailed)
            return result.ToResult();

        var token = await jwtService.GenerateUserTokenAsync(result.Value);
        return Result.Ok(new AuthLoginResponse(token));
    }
}