using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Dtos.Response
{
    public class OrderResponse
    {
        public string UserId { get; set; }
        public Guid Id { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
        public string? PaymentId { get; set; }
        public string? ShippingAddress { get; set; }

        public List<OrderItemResponse> Items { get; set; } = new();
    }
}
