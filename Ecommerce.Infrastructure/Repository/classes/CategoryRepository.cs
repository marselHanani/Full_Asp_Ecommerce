using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Application.Dtos.Response;
using Ecommerce.Domain.Entity;
using Ecommerce.Domain.Repository.Interfaces;
using Ecommerce.Infrastructure.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Repository.classes
{
    public class CategoryRepository(ApplicationDbContext context) : ICategoryRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<List<Category>> GetAllCategoriesWithProducts()
        {
            var categories = await _context.Categories.Include(p => p.Products)
                .Include(c => c.ParentCategory)
                .Include(sc => sc.SubCategories)
                .ToListAsync();
            return categories;
        }
        public async Task<Category?> GetCategoryBySlug(string slug)
        {
            var category = await _context.Categories
                .Include(p => p.Products)
                .Include(c => c.ParentCategory)
                .Include(sc => sc.SubCategories)
                .FirstOrDefaultAsync(c => c.Slug == slug);
            return category;
        }

        public async Task<Category> GetCategoryWithProductsById(Guid id)
        {
            var category = await _context.Categories
                .Include(p => p.Products)
                .Include(c => c.ParentCategory)
                .Include(sc => sc.SubCategories)
                .FirstOrDefaultAsync(c => c.Id == id);
            return category!;
        }
    }
}
