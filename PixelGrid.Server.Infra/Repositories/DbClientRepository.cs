using Microsoft.EntityFrameworkCore;
using PixelGrid.Server.Domain;
using PixelGrid.Server.Domain.Entities;
using PixelGrid.Server.Domain.Repositories;

namespace PixelGrid.Server.Infra.Repositories;

public class DbClientRepository(ApplicationDbContext dbContext) : GenericRepository<RenderClientEntity, long>(dbContext), IClientRepository
{
    public async Task<RenderClientEntity?> GetByTokenAsync(string token) =>
        await DbSet.FirstOrDefaultAsync(c => c.Token == token);

    public void SetConnected(RenderClientEntity renderClient)
    {
        renderClient.Connected = true;
        renderClient.LastConnected = DateTime.UtcNow;

        Update(renderClient);
    }

    public void SetDisconnected(RenderClientEntity renderClient)
    {
        renderClient.Connected = false;
        Update(renderClient);
    }
}