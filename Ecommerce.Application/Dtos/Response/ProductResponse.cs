using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Dtos.Response
{
    public class ProductResponse 
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Currency { get; set; } = "USD";
        public int Stock { get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
        public string MainImageUrl { get; set; } = string.Empty;
        public List<ProductImageResponse> Images { get; set; } = new();

    }
}
