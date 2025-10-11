using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Domain.Entity;
using Ecommerce.Domain.Repository.Interfaces;
using Ecommerce.Identity.Service.interfaces;
using Ecommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Repository.classes
{
    public class OrderRepository(IInformationUserService service , ApplicationDbContext context) : IOrderRepository
    {
        private readonly IInformationUserService _service = service;
        private readonly ApplicationDbContext _context = context;

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId)
        {
            var user = await _service.GetUserByIdAsync(userId);
            if (user == null) throw new InvalidOperationException("User not found");

            return await _context.Orders
                .Where(o => o.UserId == user.Id)
                .ToListAsync();
        }
        public async Task<IEnumerable<Order>> GetAllOrdersWithDetailsAsync()
        {
            return await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
                .ToListAsync();
        }
    }
}
