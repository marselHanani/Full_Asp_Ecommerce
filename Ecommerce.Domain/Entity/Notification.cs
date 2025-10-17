using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Common;

namespace Ecommerce.Domain.Entity
{
    public class Notification: BaseEntity
    {
        public string UserId { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
    }
}
