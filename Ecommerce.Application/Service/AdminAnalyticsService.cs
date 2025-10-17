using Ecommerce.Application.Dtos.Response;
using Ecommerce.Domain.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Identity.Service.interfaces;

namespace Ecommerce.Application.Service
{
    public class AdminAnalyticsService(IUnitOfWork unit, IManageUserService userService, CacheService cache)
    {
        private readonly IUnitOfWork _unit = unit;
        private readonly IManageUserService _userService = userService;
        private readonly CacheService _cache = cache;

        public async Task<AdminAnalyticsResponse> GetDashboardDataAsync()
        {
            var cacheKey = $"AdminAnalytics:Dashboard:{DateTime.UtcNow:yyyyMMdd}";
            var cachedData = await _cache.GetAsync<AdminAnalyticsResponse>(cacheKey);
            if (cachedData != null)
                return cachedData;

            var today = DateTime.UtcNow.Date;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);

            // Daily Orders
            var dailyOrders = _unit.Orders
                .Query().Where(o => o.CreatedAt.Date == today);

            // Monthly Orders
            var monthlyOrders = _unit.Orders
                .Query().Where(o => o.CreatedAt >= startOfMonth);

            // Total Revenue (paid orders)
            var totalRevenue = await _unit.Orders
                .Query()
                .Where(o => o.Payment != null && o.Payment.Status == PaymentStatus.Succeeded)
                .SumAsync(o => (decimal?)o.TotalAmount) ?? 0;

            // New Users
            var (allUsers, _) = await _userService.GetAllUsers(pageSize: int.MaxValue);
            var newUsersThisMonth = allUsers.Count(u => u.CreatedAt >= startOfMonth);
              
            // Top 5 Products
            var topProducts = await _unit.OrderItems
                .Query().Include(i => i.Product)
                .GroupBy(i => i.ProductId)
                .Select(g => new TopProductsResponse
                {
                    ProductId = g.Key,
                    TotalRevenue = g.Sum(x => x.Quantity * x.UnitPrice),
                    ProductName = g.First().Product.Name,
                    QuantitySold = g.Sum(x => x.Quantity)
                    
                })
                .OrderByDescending(x => x.QuantitySold)
                .Take(5)
                .ToListAsync();

            var response = new AdminAnalyticsResponse
            {
                DailyOrders = dailyOrders.Count(),
                MonthlyOrders = monthlyOrders.Count(),
                TotalRevenue = totalRevenue,
                NewUsersThisMonth = newUsersThisMonth,
                TopProducts = topProducts
            };

            await _cache.SetAsync(cacheKey, response, TimeSpan.FromHours(1));
            return response;
        }
    }
}
