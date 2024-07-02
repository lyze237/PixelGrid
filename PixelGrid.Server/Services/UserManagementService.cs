using FluentResults;
using PixelGrid.Server.Database.Repositories;
using PixelGrid.Shared.Models.Controller;

namespace PixelGrid.Server.Services;

public class UserManagementService(
    JwtService jwtService,
    UserManagementRepository userManagement,
    ILogger<UserManagementService> logger)
{
    public async Task<Result<AuthRegisterResponse>> Register(AuthRegisterRequest request)
    {
        logger.LogInformation("Creating user {Username}", request.UserName);

        var result = await userManagement.CreateUserAsync(request.UserName, request.Email, request.Password);
        logger.LogInformation("Created user {Username}", request.UserName);

        return result.ToResult(_ => new AuthRegisterResponse());
    }

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