using Ecommerce.Domain.Repository.Interfaces;
using Ecommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Repository.classes
{
    public class GenericRepository<TEntity>(ApplicationDbContext context)
        : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<TEntity?> GetByIdAsync(Guid id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        public async Task<IEnumerable<TEntity?>> GetAllAsync()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        public async Task AddAsync(TEntity entity)
        {
             await _context.Set<TEntity>().AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await _context.Set<TEntity>().AddRangeAsync(entities);
        }

        public void Update(Guid id ,TEntity requestEntity)
        {
            var entity = _context.Set<TEntity>().Find(id);
            if (entity is null) return;
            _context.Set<TEntity>().Update(requestEntity);
        }

        public void Delete(Guid id)
        {
            var entity = _context.Set<TEntity>().Find(id);
            if (entity is null) return;
            _context.Set<TEntity>().Remove(entity);
        }

        public IQueryable<TEntity> Query()
        {
            return _context.Set<TEntity>().AsQueryable();
        }
    }
}
