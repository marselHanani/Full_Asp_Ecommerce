using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Domain.Entity;
using Ecommerce.Domain.Repository.Interfaces;
using Ecommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Repository.classes
{
    public class ProductRepository(ApplicationDbContext context): IProductRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<List<Product>> GetAllProductsWithDetails()
        {
            return await _context.Products.Include(c => c.Category).Include(b => b.Brand)
                .Include(pi =>pi.Images).ToListAsync();
        }

        public async Task<Product?> GetProductWithCategorySlug(string slug)
        {
            return await _context.Products.Include(b => b.Brand).Include(c => c.Category)
                .Include(pi => pi.Images).FirstOrDefaultAsync(p => p.Category.Slug == slug);
        }

        public async Task<Product?> GetProductWithBrandSlug(string slug)
        {
            return await _context.Products.Include(c => c.Category).Include(b => b.Brand)
                .Include(pi => pi.Images).FirstOrDefaultAsync(p => p.Brand.Slug == slug);
        }

        public async Task<Product> GetProductWithDetailsById(Guid id)
        {
            return await _context.Products.Include(c => c.Category).Include(b => b.Brand)
                .Include(pi => pi.Images).FirstOrDefaultAsync(p => p.Id == id) ?? throw new InvalidOperationException("Product not found");
        }
    }
}
