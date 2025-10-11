using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Application.Dtos.Request;
using Ecommerce.Application.Dtos.Response;
using Ecommerce.Application.Helper;
using Ecommerce.Domain.Entity;
using Ecommerce.Domain.Repository.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Http;

namespace Ecommerce.Application.Service
{
    public class OrderService(IUnitOfWork unit, IOrderRepository orderRepo
        , PaymentService paymentService , LocationHelper location)
    {
        private readonly IUnitOfWork _unit = unit;
        private readonly IOrderRepository _orderRepo = orderRepo;
        private readonly PaymentService _paymentService = paymentService;
        private readonly LocationHelper _location = location;

        // Implement order-related business logic here

        public async Task<IEnumerable<OrderResponse>> GetAllOrders()
        {
            var orders = await _orderRepo.GetAllOrdersWithDetailsAsync();
            return orders.Adapt<IEnumerable<OrderResponse>>();
        }

        public async Task<IEnumerable<OrderResponse>> GetOrdersByUserId(string userId)
        {
            var orders = await _orderRepo.GetOrdersByUserIdAsync(userId);
            return orders.Adapt<IEnumerable<OrderResponse>>();
        }

        public async Task<Object> AddNewOrder(OrderRequest request, HttpRequest httpRequest)
        {
            var order = request.Adapt<Order>();
            order.TotalAmount = 0;

            string userIp = httpRequest.Headers["X-Forwarded-For"].FirstOrDefault()
                            ?? httpRequest.HttpContext.Connection.RemoteIpAddress?.ToString()
                            ?? "8.8.8.8";
            var currency = await _location.GetCurrencyByIpAsync(userIp);
            foreach (var item in order.Items)
            {
                var product = await _unit.Products.GetByIdAsync(item.ProductId);
                if (product == null)
                {
                    throw new ArgumentException($"Product with ID {item.ProductId} not found");
                }

                item.UnitPrice = product.Price.Amount;
                order.TotalAmount += item.Quantity * item.UnitPrice;
            }

            await _unit.Orders.AddAsync(order);

            var paymentResponse = await _paymentService.CreateCheckoutSessionAsync(
                new PaymentRequest
                {
                    OrderId = order.Id,
                    Amount = order.TotalAmount,
                    Currency = currency,
                    PaymentType = PaymentType.Cred
                }, httpRequest);

            var payment = new Payment
            {
                OrderId = order.Id,
                StripePaymentIntentId = paymentResponse.StripePaymentIntentId,
                Amount = order.TotalAmount,
                Currency = currency,
                Status = PaymentStatus.Pending,
                TransactionId = paymentResponse.CheckoutSessionId
            };

            await _unit.Payments.AddAsync(payment);

            order.PaymentId = payment.Id.ToString();

            await _unit.SaveChangesAsync();
            return new
            {
                Message = "Order created successfully.",
                OrderId = order.Id,
                PaymentUrl = paymentResponse.CheckoutUrl,
                PaymentStatus = payment.Status.ToString(),
                TotalAmount = order.TotalAmount,
                Currency = currency
            };
        }

        public async Task UpdateOrder(Guid id, OrderRequest request)
        {
            var order = request.Adapt<Order>();
            _unit.Orders.Update(id, order);
            await _unit.SaveChangesAsync();
        }
        public async Task DeleteOrder(Guid id)
        {
            _unit.Orders.Delete(id);
            await _unit.SaveChangesAsync();
        }

        public async Task<OrderResponse?> GetOrderById(Guid id)
        {
            var order = await _unit.Orders.GetByIdAsync(id);
            return order?.Adapt<OrderResponse>();
        }
    }
}
