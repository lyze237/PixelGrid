using PixelGrid.Server.Domain.Entities;

namespace PixelGrid.Server.Domain.Repositories;

public interface IRenderClientProgramVersionRepository : IGenericRepository<RenderClientProgramVersionEntity, long>
{
    Task RemoveAllFromClientAsync(RenderClientEntity client);
}