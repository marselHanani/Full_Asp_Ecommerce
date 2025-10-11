using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Dtos.Response
{
    public class CartResponse
    {
        public Guid CartId { get; set; }
        public string UserId { get; set; }
        public decimal TotalPrice { get; set; }
        public List<CartItemResponse> Items { get; set; }
    }
}
