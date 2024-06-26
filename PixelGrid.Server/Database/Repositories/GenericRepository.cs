using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace PixelGrid.Server.Database.Repositories;

public class GenericRepository<TEntity, TId>(ApplicationDbContext dbContext)
    where TEntity : class
{
    protected readonly DbSet<TEntity> DbSet = dbContext.Set<TEntity>();

    public async Task<List<TEntity>> GetAsync(Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null)
    {
        IQueryable<TEntity> query = DbSet;

        if (filter != null)
            query = query.Where(filter);

        return orderBy != null ? await orderBy(query).ToListAsync() : await query.ToListAsync();
    }

    public async Task<TEntity?> GetByIdAsync(TId id) => 
        await DbSet.FindAsync(id);

    public async Task<TEntity> CreateAsync(TEntity entity) => 
        (await DbSet.AddAsync(entity)).Entity;

    public async Task RemoveAsync(TId id)
    {
        var entity = await DbSet.FindAsync(id);
        if (entity == null)
            throw new ArgumentNullException($"Entity with id {id} not found");
        
        await Remove(entity);
    }

    public Task Remove(TEntity entity)
    {
        if (DbSet.Entry(entity).State == EntityState.Detached)
            DbSet.Attach(entity);

        DbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public Task RemoveRange(IEnumerable<TEntity> entity)
    {
        DbSet.RemoveRange(entity);
        return Task.CompletedTask;
    }

    public void Update(TEntity entity)
    {
        DbSet.Update(entity);
    }

    public async Task SaveAsync() => 
        await dbContext.SaveChangesAsync();
}