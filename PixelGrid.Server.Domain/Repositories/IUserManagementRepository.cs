using PixelGrid.Server.Domain.Entities;

namespace PixelGrid.Server.Domain.Repositories;

public interface IUserManagementRepository
{
    Task<User> CreateUserAsync(string username, string email, string password);

    Task<User> CheckUserPasswordAsync(string email, string password);

    Task<User?> FindUserByEmailAsync(string email);
}