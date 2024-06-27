using Microsoft.EntityFrameworkCore;
using PixelGrid.Server.Domain;
using PixelGrid.Server.Domain.Entities;
using PixelGrid.Server.Domain.Repositories;
using PixelGrid.Shared.Renderer;

namespace PixelGrid.Server.Infra.Repositories;

public class DbRenderClientRepository(ApplicationDbContext dbContext) : GenericRepository<RenderClientEntity, long>(dbContext), IRenderClientRepository
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