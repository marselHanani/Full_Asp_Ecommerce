using Ecommerce.Application.Dtos.Request;
using Ecommerce.Application.Dtos.Response;
using Ecommerce.Domain.Entity;
using Ecommerce.Domain.Repository.Interfaces;
using Ecommerce.Identity.Service.classes;
using Microsoft.AspNetCore.Http;
using System.Text;
using Ecommerce.Identity.Service.interfaces;

namespace Ecommerce.Application.Service
{
    public class CheckoutService(
        CartService cartService,
        OrderService orderService,
        IUnitOfWork unit,
        IInformationUserService userService)
    {
        private readonly CartService _cartService = cartService;
        private readonly OrderService _orderService = orderService;
        private readonly IUnitOfWork _unit = unit;

        public async Task<object> ProcessCheckoutAsync(CheckoutRequest request, HttpRequest httpRequest)
        {
            var cart = await _cartService.GetCartByUserIdAsync(request.UserId);
            if (cart == null || cart.Items.Count == 0)
                throw new InvalidOperationException("Cart is empty or not found.");

            // Update product stock for each item in the cart
            foreach (var item in cart.Items)
            {
                var product = await _unit.Products.GetByIdAsync(item.ProductId);
                if (product == null)
                    throw new InvalidOperationException($"Product with ID {item.ProductId} not found.");

                product.UpdateStock(-item.Quantity);
            }

            var orderRequest = new OrderRequest
            {
                UserId = request.UserId,
                Items = cart.Items.Select(i => new OrderItemRequest
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList(),
                ShippingAddress = request.ShippingAddress
            };

            var orderResult = await _orderService.AddNewOrder(orderRequest, httpRequest);

            await _cartService.ClearCartAsync(request.UserId);

            return new
            {
                Message = "Checkout started successfully.",
                OrderInfo = orderResult
            };
        }

    }
}