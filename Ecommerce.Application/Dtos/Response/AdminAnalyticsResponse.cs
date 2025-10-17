using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Dtos.Response
{
    public class AdminAnalyticsResponse
    {
        public int DailyOrders { get; set; }
        public int MonthlyOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int NewUsersThisMonth { get; set; }
        public List<TopProductsResponse> TopProducts { get; set; } = new();
    }
}
