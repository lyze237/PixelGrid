using Microsoft.EntityFrameworkCore;
using PixelGrid.Server.Domain;
using PixelGrid.Server.Domain.Entities;
using PixelGrid.Server.Domain.Repositories;
using PixelGrid.Shared.Renderer;

namespace PixelGrid.Server.Infra.Repositories;

public class DbRenderClientProgramVersionRepository(ApplicationDbContext dbContext)
    : GenericRepository<RenderClientProgramVersionEntity, long>(dbContext), IRenderClientProgramVersionRepository
{
    public async Task RemoveAllFromClientAsync(RenderClientEntity client)
    {
        var programs = await GetAsync(entity => entity.RenderClient == client);
        await RemoveRange(programs);
    }

    public List<RenderClientEntity> GetAvailableClients(RenderType type, string version)
    {
        return DbSet
            .Where(program => program.Type == type && program.Version == version)
            .Include(program => program.RenderClient)
            .Where(program => program.RenderClient.Connected)
            .Select(program => program.RenderClient)
            .Distinct()
            .ToList();
    }
}