using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PixelGrid.Server.Domain;
using PixelGrid.Server.Domain.Repositories;

namespace PixelGrid.Server.Infra.Repositories;

public class GenericRepository<TEntity, TId>(ApplicationDbContext dbContext) : IGenericRepository<TEntity, TId>
    where TEntity : class
{
    private readonly DbSet<TEntity> dbSet = dbContext.Set<TEntity>();
    
    private readonly char[] separator = [','];

    public List<TEntity> Get(Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, string includeProperties = "")
    {
        IQueryable<TEntity> query = dbSet;

        if (filter != null)
            query = query.Where(filter);

        query = includeProperties
            .Split(separator, StringSplitOptions.RemoveEmptyEntries)
            .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

        return orderBy != null ? orderBy(query).ToList() : query.ToList();
    }

    public async Task<TEntity?> GetByIdAsync(TId id) => 
        await dbSet.FindAsync(id);

    public async Task CreateAsync(TEntity entity) => 
        await dbSet.AddAsync(entity);

    public async Task RemoveAsync(TId id)
    {
        var entity = await dbSet.FindAsync(id);
        if (entity == null)
            throw new ArgumentNullException($"Entity with id {id} not found");
        
        await RemoveAsync(entity);
    }

    public Task RemoveAsync(TEntity entity)
    {
        if (dbSet.Entry(entity).State == EntityState.Detached)
            dbSet.Attach(entity);

        dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task SaveAsync() => 
        await dbContext.SaveChangesAsync();
}