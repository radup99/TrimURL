using Microsoft.EntityFrameworkCore;
using TrimUrlApi.Entities;
using TrimUrlApi.Database;

namespace TrimUrlApi.Repositories
{
    public abstract class BaseRepository<T>(MainDbContext dbContext) where T : BaseEntity
    {
        private readonly MainDbContext _dbContext = dbContext;
        private readonly DbSet<T> _dbSet = dbContext.Set<T>();

        public async Task Create(T t)
        {
            _dbSet.Add(t);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<T?> ReadById(int id)
        {
            return await _dbSet.FindAsync(id);
        }
        public async Task<List<T>?> ReadAll()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task Update(T t)
        {
            _dbSet.Update(t);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteById(int id)
        {
            _dbSet.Where(p => p.Id == id).ExecuteDelete();
            await _dbContext.SaveChangesAsync();
        }
    }
}