using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Common;

namespace Ecommerce.Domain.Entity
{
    public enum PaymentStatus
    {
        Pending = 1,
        Succeeded = 2,
        Failed = 3
    }
    public class Payment : BaseEntity
    {
        public Guid OrderId { get; set; }
        public Order? Order { get; set; }

        public string? StripePaymentIntentId { get; set; } 
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";

        public PaymentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string TransactionId { get; set; }
    }
}
