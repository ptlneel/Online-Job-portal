using Microsoft.EntityFrameworkCore;
using OnlineJobPortal.Data;
using OnlineJobPortal.Repositories.Interfaces;
using System.Linq.Expressions;

namespace OnlineJobPortal.Repositories.Implementations;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null)
    {
        var query = _dbSet.AsQueryable();
        if (predicate != null) query = query.Where(predicate);
        return await query.ToListAsync();
    }

    public Task<T?> GetByIdAsync(object id) => _dbSet.FindAsync(id).AsTask();

    public Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        => _dbSet.FirstOrDefaultAsync(predicate);

    public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

    public void Update(T entity) => _dbSet.Update(entity);

    public void Delete(T entity) => _dbSet.Remove(entity);

    public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();
}
