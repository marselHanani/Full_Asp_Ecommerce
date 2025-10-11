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
    public class BrandRepository(ApplicationDbContext context) : IBrandRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<List<Brand>> GetBrandsWithProducts()
        {
            var brands = await _context.Brands.Include(p => p.Products).ToListAsync();
            return brands;
        }

        public async Task<Brand> GetBrandByIdWithProducts(Guid id)
        {
            var brand = await _context.Brands.Include(p => p.Products)
                .FirstOrDefaultAsync(b => b.Id == id);
            return brand!;
        }
    }
}
