using PixelGrid.Server.Domain.Entities;
using PixelGrid.Shared.Renderer;

namespace PixelGrid.Server.Domain.Repositories;

public interface IProjectRepository : IGenericRepository<ProjectEntity, long>
{
}