using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Domain.Entity;

namespace Ecommerce.Domain.Repository.Interfaces
{
    public interface ICategoryRepository
    {
        // Define category-specific methods here if needed
        Task<List<Category>> GetAllCategoriesWithProducts();
        Task<Category?> GetCategoryBySlug(string slug);
        Task<Category> GetCategoryWithProductsById(Guid id);
    }
}
