using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Dtos.Response
{
    public class BrandResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string ImageUrl { get; set; }
        public List<ProductResponse> Products { get; set; } = new List<ProductResponse>();
    }
}
