using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Ecommerce.Application.Dtos.Request
{
    public enum PaymentType
    {
        Cred = 1,
        PayPal = 2,
        PayOnDelivery = 3
    }
    public class OrderRequest
    {
        [JsonIgnore]
        public string? UserId { get; set; }
        public List<OrderItemRequest> Items { get; set; } = new();
        public string ShippingAddress { get; set; }
        public PaymentType PaymentMethod { get; set; }
    }
}
