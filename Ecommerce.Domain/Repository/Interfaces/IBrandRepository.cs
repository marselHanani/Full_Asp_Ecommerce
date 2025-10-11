using Ecommerce.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Domain.Repository.Interfaces
{
    public interface IBrandRepository
    {
        Task<Brand> GetBrandByIdWithProducts(Guid id);
        Task<List<Brand>> GetBrandsWithProducts();
    }
}
