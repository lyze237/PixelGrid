using PixelGrid.Server.Domain.Entities;

namespace PixelGrid.Server.Domain.Repositories;

public interface IUserManagementRepository
{
    Task<UserEntity> CreateUserAsync(string username, string email, string password);

    Task<UserEntity> CheckUserPasswordAsync(string email, string password);

    Task<UserEntity?> FindUserByEmailAsync(string email);
}