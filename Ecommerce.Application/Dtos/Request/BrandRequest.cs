using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Ecommerce.Application.Dtos.Request
{
    public class BrandRequest
    {
        public string Name { get; set; }
        public IFormFile Image { get; set; }
    }
}
