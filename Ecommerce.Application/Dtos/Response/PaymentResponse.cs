using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Dtos.Response
{
    public class PaymentResponse
    {
        public Guid OrderId { get; set; }
        public bool IsSuccess { get; set; }
        public string StripePaymentIntentId { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public string CheckoutSessionId { get; set; }
        public string CheckoutUrl { get; set; }
    }
}
