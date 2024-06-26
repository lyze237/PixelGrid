using PixelGrid.Server.Domain;
using PixelGrid.Server.Domain.Entities;
using PixelGrid.Server.Domain.Repositories;

namespace PixelGrid.Server.Infra.Repositories;

public class DbClientRepository(ApplicationDbContext dbContext) : GenericRepository<ClientEntity, long>(dbContext), IClientRepository
{
    
}