using Ecommerce.Application.Dtos.Response;
using Ecommerce.Domain.Entity;
using Ecommerce.Domain.Repository.Interfaces;
using Ecommerce.Identity.Service.interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Service
{
    public class ReportingService(IUnitOfWork unitOfWork , IInformationUserService userServ)
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IInformationUserService _userServ = userServ;

        public async Task<List<SalesReportResponse>> GetSalesReportAsync(DateTime from, DateTime to)
        {
            var orders = await _unitOfWork.Orders
                .Query()
                .Where(o => o.OrderDate >= from && o.OrderDate <= to)
                .Include(o => o.Items)
                .ToListAsync();

            return orders
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new SalesReportResponse
                {
                    Date = g.Key,
                    TotalOrders = g.Count(),
                    TotalRevenue = g.Sum(o => o.TotalAmount),
                    PendingOrders = g.Count(o => o.Status == StatusType.Pending),
                    ShippedOrders = g.Count(o => o.Status == StatusType.Shipped),
                    DeliveredOrders = g.Count(o => o.Status == StatusType.Delivered),
                    CancelledOrders = g.Count(o => o.Status == StatusType.Cancelled)
                })
                .OrderBy(r => r.Date)
                .ToList();
        }

        public async Task<List<TopProductsResponse>> GetTopProductsAsync(int top = 10)
        {
            var products = await _unitOfWork.OrderItems
                .Query()
                .Include(oi => oi.Product)
                .GroupBy(oi => new { oi.ProductId, oi.Product.Name })
                .Select(g => new TopProductsResponse
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name,
                    QuantitySold = g.Sum(oi => oi.Quantity),
                    TotalRevenue = g.Sum(oi => oi.Quantity * oi.TotalPrice)
                })
                .OrderByDescending(p => p.QuantitySold)
                .Take(top)
                .ToListAsync();

            return products;
        }

        public async Task<List<TopCustomersResponse>> GetTopCustomersAsync(int top = 10)
        {
            var groupedData = await _unitOfWork.Orders
                .Query()
                .GroupBy(o => o.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    OrdersCount = g.Count(),
                    TotalSpent = g.Sum(o => o.TotalAmount)
                })
                .OrderByDescending(x => x.TotalSpent)
                .Take(top)
                .ToListAsync();

            var topCustomers = new List<TopCustomersResponse>();

            foreach (var item in groupedData)
            {
                var user = await _userServ.GetUserByIdAsync(item.UserId);

                if (user != null)
                {
                    topCustomers.Add(new TopCustomersResponse
                    {
                        UserId = item.UserId,
                        FullName = user.FullName,
                        OrdersCount = item.OrdersCount,
                        TotalSpent = item.TotalSpent
                    });
                }
            }

            return topCustomers;
        }
    }
}
