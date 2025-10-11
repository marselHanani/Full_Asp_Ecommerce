using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Domain.Repository.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class 
    {
        Task<TEntity?> GetByIdAsync(Guid id);
        Task<IEnumerable<TEntity?>> GetAllAsync();
        Task AddAsync(TEntity entity);
        Task AddRangeAsync(IEnumerable<TEntity> entities);
        void Update(Guid id,TEntity entity);
        void Delete(Guid id);

    }
}
