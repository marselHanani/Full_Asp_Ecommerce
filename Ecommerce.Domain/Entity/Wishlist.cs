using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace Ecommerce.Domain.Entity
{
    public class Wishlist
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public Guid ProductId { get; set; }
        public DateTime DateAdded { get; set; }
    }
}