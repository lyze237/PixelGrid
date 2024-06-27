using PixelGrid.Server.Domain;
using PixelGrid.Server.Domain.Entities;
using PixelGrid.Server.Domain.Repositories;

namespace PixelGrid.Server.Infra.Repositories;

public class DbRenderClientProgramVersionRepository(ApplicationDbContext dbContext)
    : GenericRepository<RenderClientProgramVersionEntity, long>(dbContext), IRenderClientProgramVersionRepository
{
    public async Task RemoveAllFromClientAsync(RenderClientEntity client)
    {
        var programs = await GetAsync(entity => entity.RenderClient == client);
        await RemoveRange(programs);
    }
}