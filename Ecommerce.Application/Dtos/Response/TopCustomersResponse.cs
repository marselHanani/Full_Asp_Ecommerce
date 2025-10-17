using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Dtos.Response
{
    public class TopCustomersResponse
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public int OrdersCount { get; set; }
        public decimal TotalSpent { get; set; }
    }
}
