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
    public class CartRepository(IInformationUserService userService, ApplicationDbContext context) : ICartRepository
    {
        private readonly IInformationUserService _userService = userService;
        private readonly ApplicationDbContext _context = context;

        public async Task<Cart> GetCartByUserIdAsync(string userId)
        {
            return (await _context.Carts.Include(it => it.Items).ThenInclude(u => u.Product).FirstOrDefaultAsync(c => c.UserId == userId))!;
        }

        public async Task ClearCartAsync(string userId)
        {
            var cart = await GetCartByUserIdAsync(userId);
            if (cart != null)
            {
                _context.CartItems.RemoveRange(cart.Items);
                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();
            }
        }
    }
}
