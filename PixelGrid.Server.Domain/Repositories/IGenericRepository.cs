using System.Linq.Expressions;

namespace PixelGrid.Server.Domain.Repositories;

public interface IGenericRepository<TEntity, TId> where TEntity : class 
{
    List<TEntity> Get(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        string includeProperties = "");
    Task<TEntity?> GetByIdAsync(TId id);
    Task CreateAsync(TEntity entity);
    Task RemoveAsync(TId id);
    Task RemoveAsync(TEntity entity);
    Task SaveAsync();
}