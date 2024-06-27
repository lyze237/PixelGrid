using PixelGrid.Server.Domain.Entities;

namespace PixelGrid.Server.Domain.Repositories;

public interface IClientRepository : IGenericRepository<RenderClientEntity, long>
{
    Task<RenderClientEntity?> GetByTokenAsync(string token);
    void SetConnected(RenderClientEntity renderClient);
    void SetDisconnected(RenderClientEntity renderClient);
}