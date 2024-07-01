using PixelGrid.Server.Database.Entities;

namespace PixelGrid.Server.Database.Repositories;

public class ProjectRepository(ApplicationDbContext dbContext) : GenericRepository<ProjectEntity, long>(dbContext)
{
}