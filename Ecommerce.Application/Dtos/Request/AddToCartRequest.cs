using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Ecommerce.Application.Dtos.Request
{
    public class AddToCartRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }

    }
}
