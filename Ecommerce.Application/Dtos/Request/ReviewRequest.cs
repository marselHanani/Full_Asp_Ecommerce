using Ecommerce.Domain.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Dtos.Request
{
    public class ReviewRequest
    {
        public Guid ProductId { get; set; }
        public Guid UserId { get; set; }
        [Range(maximum: 5, minimum: 1)]
        public int Rating { get; set; } 
        public string Title { get; set; }
        public string Content { get; set; }

    }
}
