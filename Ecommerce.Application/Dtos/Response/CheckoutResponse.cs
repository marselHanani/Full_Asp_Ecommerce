using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Dtos.Response
{
    public class CheckoutResponse
    {
        public Guid OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentIntentId { get; set; }
        public string Message { get; set; }
    }
}
