using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Common;

namespace Ecommerce.Domain.Entity
{
    public class Brand : BaseEntity
    {
        public string Name { get; set; }
        public string Slug { get; set; }

        public string ImageName { get; set; } = string.Empty;

        public ICollection<Product> Products { get; private set; } = new List<Product>();
    }
}
