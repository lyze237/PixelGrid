using Grpc.Core;
using Microsoft.AspNetCore.Identity;
using PixelGrid.Server.Db;
using PixelGrid.Server.Services.Jwt;

namespace PixelGrid.Server.Controllers;

public class AuthController(IJwtService jwtService, UserManager<User> userManager, RoleManager<Role> roleManager, ILogger<AuthController> logger) : Auth.AuthBase
{
    public override async Task<AuthRegisterResponse> Register(AuthRegisterRequest request, ServerCallContext context)
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
            
            return new AuthRegisterResponse { Success = true };
        }

        logger.LogError("Failed to create user: {errors}", string.Join(", ", result.Errors.Select(e => $"{e.Code} {e.Description}")));
        return new AuthRegisterResponse { Success = false };
    }

    public override async Task<AuthLoginResponse> Login(AuthLoginRequest request, ServerCallContext context)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return new AuthLoginResponse {Success = false};

        var result = await userManager.CheckPasswordAsync(user, request.Password);
        if (result)
            return new AuthLoginResponse
            {
                Success = true,
                Token = await jwtService.GenerateTokenAsync(user)
            };
        
        return new AuthLoginResponse {Success = false};
    }
}