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
    public class ReviewRepository(ApplicationDbContext context) : IReviewRepository
    {
        private readonly ApplicationDbContext _context = context;
        
        public async Task<IEnumerable<Review>> GetAllReviewsByProductId(Guid productId)
        {
            return await _context.Reviews
                .Include(r => r.Product)
                .Include(r => r.Reviews)
                    .ThenInclude(reply => reply.Reviews) // Load nested replies too
                .Where(r => r.ProductId == productId && r.ParentReviewId == null) // Only parent reviews
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetAllReviewsByUserId(Guid userId)
        {
            return await _context.Reviews
                .Include(r => r.Product)
                .Include(r => r.Reviews)
                    .ThenInclude(reply => reply.Reviews) // Load nested replies too
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetAllReviews()
        {
            return await _context.Reviews
                .Include(r => r.Product)
                .Include(r => r.Reviews)
                    .ThenInclude(reply => reply.Reviews) // Load nested replies too
                .Where(r => r.ParentReviewId == null) // Only parent reviews
                .ToListAsync();
        }
    }
}
