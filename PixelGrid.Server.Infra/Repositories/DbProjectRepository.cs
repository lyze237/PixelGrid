using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using PixelGrid.Server.Domain;
using PixelGrid.Server.Domain.Entities;
using PixelGrid.Server.Domain.Repositories;
using PixelGrid.Shared.Renderer;

namespace PixelGrid.Server.Infra.Repositories;

public class DbProjectRepository(ApplicationDbContext dbContext) : GenericRepository<ProjectEntity, long>(dbContext), IProjectRepository
{
}