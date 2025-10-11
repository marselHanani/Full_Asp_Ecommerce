using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Ecommerce.Application.Dtos.Request
{
    public class ProductImageRequest
    {
        public Guid ProductId { get; set; }
        public IFormFile Image { get; set; }

        public bool IsMain { get; set; }
    }
}
