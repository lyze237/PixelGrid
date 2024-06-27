using System.Linq.Expressions;

namespace PixelGrid.Server.Domain.Repositories;

public interface IGenericRepository<TEntity, TId> where TEntity : class 
{
    Task<List<TEntity>> GetAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null);
    Task<TEntity?> GetByIdAsync(TId id);
    Task<TEntity> CreateAsync(TEntity entity);
    Task RemoveAsync(TId id);
    Task Remove(TEntity entity);
    Task RemoveRange(IEnumerable<TEntity> entity);
    public void Update(TEntity entity);
    Task SaveAsync();
}