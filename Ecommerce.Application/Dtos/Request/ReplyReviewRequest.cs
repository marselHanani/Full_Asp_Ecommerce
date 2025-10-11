using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Dtos.Request
{
    public class ReplyReviewRequest
    {
        public string Comment { get; set; }
        public Guid ParentId { get; set; }
    }
}
