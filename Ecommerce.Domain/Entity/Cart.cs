using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Common;

namespace Ecommerce.Domain.Entity
{
    public class Cart : BaseEntity
    {
        public string UserId { get; set; }
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
        public decimal TotalPrice { get; set; }
    }
}
