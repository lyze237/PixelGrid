using System.Security.Authentication;
using Microsoft.AspNetCore.Identity;
using PixelGrid.Server.Domain.Entities;
using PixelGrid.Server.Domain.Repositories;
using PixelGrid.Server.Infra.Exceptions;

namespace PixelGrid.Server.Infra.Repositories;

public class DbUserManagementRepository(UserManager<UserEntity> userManager, RoleManager<RoleEntity> roleManager) : IUserManagementRepository
{
    public async Task<UserEntity> CreateUserAsync(string username, string email, string password)
    {
        var user = new UserEntity
        {
            UserName = username,
            Email = email
        };

        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded)
            return user;

        throw new CreateUserException("Couldn't create user", result.Errors);
    }

    public async Task<UserEntity> CheckUserPasswordAsync(string email, string password)
    {
        var user = await FindUserByEmailAsync(email);
        if (user == null)
            throw new EntityNotFoundException<UserEntity>("Couldn't find user by email");

        if (await userManager.CheckPasswordAsync(user, password))
            return user;

        throw new InvalidCredentialException();
    }

    public async Task<UserEntity?> FindUserByEmailAsync(string email) => 
        await userManager.FindByEmailAsync(email);
}