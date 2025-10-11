using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace Ecommerce.Application.Dtos.Request
{
    public class CheckoutRequest
    {
        [JsonIgnore]
        public string? UserId { get; set; }
        public string ShippingAddress { get; set; }
    }
}
