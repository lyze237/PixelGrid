using PixelGrid.Server.Domain.Entities;
using PixelGrid.Shared.Renderer;

namespace PixelGrid.Server.Domain.Repositories;

public interface IRenderClientProgramVersionRepository : IGenericRepository<RenderClientProgramVersionEntity, long>
{
    Task RemoveAllFromClientAsync(RenderClientEntity client);
    List<RenderClientEntity> GetAvailableClients(RenderType type, string version);
}