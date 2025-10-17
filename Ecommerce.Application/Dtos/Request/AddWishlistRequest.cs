using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Dtos.Request
{
    public class AddWishlistRequest
    {
        public Guid ProductId { get; set; }
    }
}
