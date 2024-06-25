using Grpc.Core;
using Microsoft.AspNetCore.Identity;
using PixelGrid.Server.Db;
using PixelGrid.Server.Services.Jwt;

namespace PixelGrid.Server.Controllers;

public class AuthController(IJwtService jwtService, UserManager<User> userManager, RoleManager<Role> roleManager, ILogger<AuthController> logger) : Auth.AuthBase
{
    public override async Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext context)
    {
        if (await roleManager.FindByNameAsync("Role") == null)
        {
            await roleManager.CreateAsync(new Role
            {
                Name = "Test"
            });
        }

        logger.LogInformation("Creating user {username}", request.UserName);
        
        var result = await userManager.CreateAsync(new User
        {
            UserName = request.UserName,
            Email = request.Email,
        }, request.Password);
        
        if (result.Succeeded)
        {
            logger.LogInformation("Created user {username}", request.UserName);

            var user = await userManager.FindByEmailAsync(request.Email);
            await userManager.AddToRoleAsync(user, "Test");
            
            return new RegisterResponse { Success = true };
        }

        logger.LogError("Failed to create user: {errors}", string.Join(", ", result.Errors.Select(e => $"{e.Code} {e.Description}")));
        return new RegisterResponse { Success = false };
    }

    public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return new LoginResponse {Success = false};

        var result = await userManager.CheckPasswordAsync(user, request.Password);
        if (result)
            return new LoginResponse
            {
                Success = true,
                Token = await jwtService.GenerateTokenAsync(user)
            };
        
        return new LoginResponse {Success = false};
    }
}