using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Dtos.Request
{
    public class ProductRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Currency { get; set; } = "USD";
        public int Stock { get; set; }
        public Guid CategoryId { get; set; }
        public Guid BrandId { get; set; }
        public IFormFile? MainImage { get; set; }
        public ICollection<IFormFile>? SubImages { get; set; } = new List<IFormFile>();
    }
}
