using PixelGrid.Server.Domain.Repositories;

namespace PixelGrid.Server.Services;

public class UserManagementService(JwtService jwtService, IUserManagementRepository userManagement, ILogger<UserManagementService> logger)
{
    public async Task<AuthRegisterResponse> Register(AuthRegisterRequest request)
    {
        logger.LogInformation("Creating user {Username}", request.UserName);

        await userManagement.CreateUserAsync(request.UserName, request.Email, request.Password);
        logger.LogInformation("Created user {Username}", request.UserName);

        return new AuthRegisterResponse { Success = true };
    }
    
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