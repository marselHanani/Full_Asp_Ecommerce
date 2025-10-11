using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Common;

namespace Ecommerce.Domain.Entity
{
    public class ProductImage : BaseEntity
    {
        public Guid ProductId { get; set; }
        public string FileName { get; set; }
        public bool IsMain { get; set; }

        public Product Product { get; private set; } = null!;
    }
}
