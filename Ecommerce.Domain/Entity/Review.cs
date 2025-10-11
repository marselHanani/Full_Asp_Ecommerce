using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Common;

namespace Ecommerce.Domain.Entity
{
    public class Review : BaseEntity
    {
        public Guid ProductId { get; set; }
        public Guid UserId { get; set; }
        [Range(maximum:5,minimum:1)]
        public int Rating { get; set; } // 1 - 5
        public string Title { get; set; }
        public string Content { get; set; }

        public Guid? ParentReviewId { get; set; }
        
        // Navigation properties
        public Review? ParentReview { get; set; }
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public Product Product { get; private set; } = null!;
    }
}
