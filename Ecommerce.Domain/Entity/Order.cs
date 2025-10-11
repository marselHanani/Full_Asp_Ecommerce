using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Common;

namespace Ecommerce.Domain.Entity
{
    public enum StatusType
    {
        Pending = 1,
        Processing = 2,
        Shipped = 3,
        Delivered = 4,
        Cancelled = 5,
    }
    public class Order : BaseEntity
    {
        public string UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public StatusType Status { get; set; } = StatusType.Pending;
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public string? PaymentId { get; set; }
        public Payment? Payment { get; set; }
        public string? ShippingAddress { get; set; }

        public List<OrderItem> Items { get; set; } = new();


    }
}
