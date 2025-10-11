using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Domain.Entity;

namespace Ecommerce.Domain.Repository.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProductsWithDetails();
        Task<Product?> GetProductWithCategorySlug(string slug);
        Task<Product?> GetProductWithBrandSlug(string slug);

        Task<Product> GetProductWithDetailsById(Guid id);
    }
}
