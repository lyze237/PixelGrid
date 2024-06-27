using PixelGrid.Server.Domain.Entities;

namespace PixelGrid.Server.Domain.Repositories;

public interface IRenderClientRepository : IGenericRepository<RenderClientEntity, long>
{
    Task<RenderClientEntity?> GetByTokenAsync(string token);
    void SetConnected(RenderClientEntity renderClient, string connectionId);
    void SetDisconnected(RenderClientEntity renderClient);
}