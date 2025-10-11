using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Domain.Entity;

namespace Ecommerce.Domain.Repository.Interfaces
{
    public interface IReviewRepository
    {
        Task<IEnumerable<Review>> GetAllReviewsByProductId(Guid productId);
        Task<IEnumerable<Review>> GetAllReviewsByUserId(Guid userId);
        Task<IEnumerable<Review>> GetAllReviews();
    }
}
