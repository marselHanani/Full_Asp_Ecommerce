using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Application.Dtos.Request;
using Ecommerce.Application.Dtos.Response;
using Ecommerce.Domain.Entity;
using Ecommerce.Domain.Repository.Interfaces;
using Mapster;

namespace Ecommerce.Application.Service
{
    public class CartService(IUnitOfWork unit, ICartRepository cartRepo)
    {
        private readonly IUnitOfWork _unit = unit;
        private readonly ICartRepository _cartRepo = cartRepo;

        public async Task<CartResponse> GetCartByUserIdAsync(string userId)
        {
            var cart = await _cartRepo.GetCartByUserIdAsync(userId);
            return cart.Adapt<CartResponse>();
        }

        public async Task ClearCartAsync(string userId)
        {
            await _cartRepo.ClearCartAsync(userId);
        }

        public async Task<CartResponse> AddItemToCartAsync(AddToCartRequest request , string userId)
        {
            var cart = await _cartRepo.GetCartByUserIdAsync(userId);
            var product = await _unit.Products.GetByIdAsync(request.ProductId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    Items = new List<CartItem>(),
                };
            }

            var cartItem = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
            if (cartItem != null)
            {
                cartItem.Quantity += request.Quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    UnitPrice = product.Price.Amount
                });
            }

            cart.TotalPrice = cart.Items.Sum(s => s.Total);
            await _unit.Carts.AddAsync(cart);
            await _unit.SaveChangesAsync();
            return await GetCartByUserIdAsync(userId);
        }

        public async Task RemoveCartItemAsync(RemoveCartItemRequest request)
        {
            var cart = await _cartRepo.GetCartByUserIdAsync(request.UserId);
            var cartItem = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
            if (cartItem != null)
            {
                cart.Items.Remove(cartItem);
                cart.TotalPrice = cart.Items.Sum(s => s.Total);
                _unit.Carts.Update(cart.Id, cart);
                await _unit.SaveChangesAsync();
            }
        }
    }
}
