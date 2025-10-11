using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Dtos.Request
{
    public class CategoryRequest
    {
        public Guid? ParentCategoryId { get; set; }
        public string Name { get; set; }
    }
}
