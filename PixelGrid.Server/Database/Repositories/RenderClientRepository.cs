using Microsoft.EntityFrameworkCore;
using PixelGrid.Server.Database.Entities;

namespace PixelGrid.Server.Database.Repositories;

public class RenderClientRepository(ApplicationDbContext dbContext) : GenericRepository<RenderClientEntity, long>(dbContext)
{
    public async Task<RenderClientEntity?> GetByTokenAsync(string token) =>
        await DbSet.FirstOrDefaultAsync(c => c.Token == token);

    public void SetConnected(RenderClientEntity renderClient, string connectionId)
    {
        renderClient.Connected = true;
        renderClient.LastConnected = DateTime.UtcNow;
        renderClient.ConnectionId = connectionId;

        Update(renderClient);
    }

    public void SetDisconnected(RenderClientEntity renderClient)
    {
        renderClient.Connected = false;
        renderClient.ConnectionId = null;
        
        Update(renderClient);
    }
}