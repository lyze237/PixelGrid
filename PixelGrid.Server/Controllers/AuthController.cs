using System.Security.Authentication;
using Grpc.Core;
using Microsoft.AspNetCore.Identity;
using PixelGrid.Server.Domain.Entities;
using PixelGrid.Server.Infra.Exceptions;
using PixelGrid.Server.Infra.Repositories;
using PixelGrid.Server.Services.Jwt;

namespace PixelGrid.Server.Controllers;

public class AuthController(IJwtService jwtService, DbUserManagementRepository userManagement, ILogger<AuthController> logger) : Auth.AuthBase
{
    public override async Task<AuthRegisterResponse> Register(AuthRegisterRequest request, ServerCallContext context)
    {
        logger.LogInformation("Creating user {username}", request.UserName);

        try
        {
            await userManagement.CreateUserAsync(request.UserName, request.Email, request.Password);
            logger.LogInformation("Created user {username}", request.UserName);

            return new AuthRegisterResponse { Success = true };
        }
        catch (CreateUserException e)
        {
            logger.LogError("Failed to create user: {errors}", string.Join(", ", e.Errors.Select(identityError => $"{identityError.Code} {identityError.Description}")));
            return new AuthRegisterResponse { Success = false };
        }
    }

    public override async Task<AuthLoginResponse> Login(AuthLoginRequest request, ServerCallContext context)
    {
        logger.LogInformation("User {email} tries to login", request.Email);
        try
        {
            var user = await userManagement.CheckUserPasswordAsync(request.Email, request.Password);
            logger.LogInformation("User {email} logged in successfully", request.Email);
            
            return new AuthLoginResponse
            {
                Success = true,
                Token = await jwtService.GenerateTokenAsync(user)
            };
        }
        catch (EntityNotFoundException<User>)
        {
            logger.LogInformation("User {email} couldn't login as the email doesn't exist", request.Email);
            return new AuthLoginResponse { Success = false };
        }
        catch (InvalidCredentialException)
        {
            logger.LogInformation("User {email} couldn't login as the password is wrong", request.Email);
            return new AuthLoginResponse { Success = false };
        }
    }
}