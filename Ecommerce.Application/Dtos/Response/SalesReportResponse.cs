using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Dtos.Response
{
    public class SalesReportResponse
    {
        public DateTime Date { get; set; }
        public long TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }

        public int PendingOrders { get; set; }
        public int ShippedOrders { get; set; }
        public int DeliveredOrders { get; set; }
        public int CancelledOrders { get; set; }
        public int PaidOrders { get; set; }
    }
}
