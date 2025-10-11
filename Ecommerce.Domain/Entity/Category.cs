using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Common;

namespace Ecommerce.Domain.Entity
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public string Slug { get; set; }

        public Guid? ParentCategoryId { get; set; }

        public Category? ParentCategory { get; set; }

        public ICollection<Category> SubCategories { get; set; } = new List<Category>();
        public ICollection<Product> Products { get; set; } = new List<Product>();

    }
}
