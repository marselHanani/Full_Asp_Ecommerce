using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Dtos.Response
{
    public class CategoryResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public ICollection<ProductResponse>? Products { get; set; }
        public ICollection<CategoryResponse>? SubCategories { get; set; }
        public CategoryResponse? ParentCategory { get; set; }

    }
}
