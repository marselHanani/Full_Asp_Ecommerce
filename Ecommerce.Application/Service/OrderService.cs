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
        , PaymentService paymentService , LocationHelper location , NotificationService notification ,CacheService cache)
    {
        private readonly IUnitOfWork _unit = unit;
        private readonly IOrderRepository _orderRepo = orderRepo;
        private readonly PaymentService _paymentService = paymentService;
        private readonly LocationHelper _location = location;
        private readonly NotificationService _notification = notification;
        private readonly CacheService _cache = cache;

        // Implement order-related business logic here

        public async Task<(IEnumerable<OrderResponse> Orders, int TotalCount)> GetAllOrders(
            string? search = null,
            string? status = null,
            int page = 1,
            int pageSize = 10)
        {
            string cacheKey = $"orders_{search}_{status}_{page}_{pageSize}";
            var cachedResult = await _cache.GetAsync<IEnumerable<OrderResponse>>(cacheKey);
            if (cachedResult != null) return (cachedResult, cachedResult.Count());
          

            var orders = await _orderRepo.GetAllOrdersWithDetailsAsync();

            // Filtering
            if (!string.IsNullOrWhiteSpace(status))
            {
                orders = orders.Where(o => o.Status.ToString().Equals(status, StringComparison.OrdinalIgnoreCase));
            }

            // Searching (by UserId or ShippingAddress)
            if (!string.IsNullOrWhiteSpace(search))
            {
                orders = orders.Where(o =>
                    (!string.IsNullOrEmpty(o.UserId) && o.UserId.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(o.ShippingAddress) && o.ShippingAddress.Contains(search, StringComparison.OrdinalIgnoreCase))
                );
            }

            int totalCount = orders.Count();

            // Pagination
            orders = orders
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            var result = orders.Adapt<IEnumerable<OrderResponse>>();
            var response = (result, totalCount);

            await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5));
            return response;
        }

        public async Task<IEnumerable<OrderResponse>> GetOrdersByUserId(Guid userId)
        {
            var orders = await _orderRepo.GetOrdersByUserIdAsync(userId.ToString());
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
            await _notification.SendNotificationAsync(order.UserId,$"Your order {order} has been created successfully.");
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
