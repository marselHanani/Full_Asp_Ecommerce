using Ecommerce.Application.Dtos.Request;
using Ecommerce.Application.Dtos.Response;
using Ecommerce.Domain.Entity;
using Ecommerce.Identity.Service.interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Stripe;
using Stripe.Checkout;
using Ecommerce.Domain.Repository.Interfaces;

namespace Ecommerce.Application.Service
{
    public class PaymentService
    {
        private readonly IEmailService _emailService;
        private readonly IInformationUserService _userService;
        private readonly IUnitOfWork _unit;
        private readonly NotificationService _notification;

        private readonly StripeSettings _stripeSettings;
        public PaymentService(IOptions<StripeSettings> stripeSettings, 
            IEmailService emailService , 
            IInformationUserService userService,
            IUnitOfWork unit,
            NotificationService notification)
        {
            _emailService = emailService;
            _userService = userService;
            _unit = unit;
            _notification = notification;
            _stripeSettings = stripeSettings.Value;
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
        }

        public async Task<PaymentResponse> CreateCheckoutSessionAsync(PaymentRequest request, HttpRequest httpRequest)
        {
            
            string successUrl = $"{httpRequest.Scheme}://{httpRequest.Host}/api/Customer/Payments/checkout/success?session_id={{CHECKOUT_SESSION_ID}}&order_id={request.OrderId}";
            string cancelUrl = $"{httpRequest.Scheme}://{httpRequest.Host}/api/Customer/Payments/checkout/cancel?order_id={request.OrderId}";

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(request.Amount * 100),
                            Currency = request.Currency ?? "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Order Payment",
                            },
                        },
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
                ClientReferenceId = request.OrderId.ToString()
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            return new PaymentResponse
            {
                OrderId = request.OrderId,
                IsSuccess = true,
                StripePaymentIntentId = session.PaymentIntentId,
                CheckoutSessionId = session.Id,
                CheckoutUrl = session.Url ?? $"https://checkout.stripe.com/pay/{session.Id}",
                Status = session.Status,
                Message = "Checkout session created successfully"
            };
        }

        public async Task<PaymentResponse> GetCheckoutSessionStatusAsync(string sessionId)
        {
            var service = new SessionService();
            var session = await service.GetAsync(sessionId);

            bool isSuccess = session.PaymentStatus == "paid";
            string message = isSuccess ? "Payment succeeded." : "Payment not completed.";
            Guid orderId = Guid.Parse(session.ClientReferenceId);

            var order = await _unit.Orders.GetByIdAsync(orderId);
            if (order != null)
            {
                var user = await _userService.GetUserByIdAsync(order.UserId);
                if (user != null && !string.IsNullOrEmpty(user.Email))
                {
                    string subject = "Payment Successful";

                    // Build product list HTML
                    string productRows = "";
                    if (order.Items != null && order.Items.Count > 0)
                    {
                        foreach (var item in order.Items)
                        {
                            productRows += $@"
                                <tr>
                                    <td style='padding:8px;border:1px solid #ddd;'>{item.ProductId}</td>
                                    <td style='padding:8px;border:1px solid #ddd;'>{item.UnitPrice:C}</td>
                                    <td style='padding:8px;border:1px solid #ddd;'>{item.TotalPrice:C}</td>
                                </tr>";
                        }
                    }

                    string htmlContent = $@"
                        <html>
                        <head>
                            <style>
                                body {{
                                    font-family: 'Segoe UI', Arial, sans-serif;
                                    background-color: #f4f4f4;
                                    margin: 0;
                                    padding: 0;
                                }}
                                .container {{
                                    background: #fff;
                                    max-width: 600px;
                                    margin: 40px auto;
                                    border-radius: 8px;
                                    box-shadow: 0 2px 8px rgba(0,0,0,0.05);
                                    padding: 32px;
                                }}
                                h2 {{
                                    color: #2d7ff9;
                                }}
                                table {{
                                    width: 100%;
                                    border-collapse: collapse;
                                    margin-top: 20px;
                                }}
                                th, td {{
                                    text-align: left;
                                }}
                                th {{
                                    background: #f0f4fa;
                                    color: #333;
                                }}
                                .total {{
                                    font-weight: bold;
                                    color: #2d7ff9;
                                }}
                                .footer {{
                                    margin-top: 32px;
                                    font-size: 13px;
                                    color: #888;
                                    text-align: center;
                                }}
                            </style>
                        </head>
                        <body>
                            <div class='container'>
                                <h2>Thank you for your payment!</h2>
                                <p>Dear {(user.FullName ?? user.UserName)},</p>
                                <p>Your payment for order <b>{orderId}</b> was successful.</p>
                                <h3>Order Details</h3>
                                <table>
                                    <thead>
                                        <tr>
                                            <th style='padding:8px;border:1px solid #ddd;'>Product ID</th>
                                            <th style='padding:8px;border:1px solid #ddd;'>Unit Price</th>
                                            <th style='padding:8px;border:1px solid #ddd;'>Total Price</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {productRows}
                                    </tbody>
                                </table>
                                <p class='total' style='margin-top:20px;'>Total Amount: {order.TotalAmount:C}</p>
                                <p style='margin-top:20px;'>Order Date: {order.OrderDate:yyyy-MM-dd HH:mm}</p>
                                <p>Shipping Address: {order.ShippingAddress}</p>
                                <div class='footer'>
                                    If you have any questions, please contact our support.<br/>
                                    &copy; {DateTime.Now.Year} Ecommerce Team
                                </div>
                            </div>
                        </body>
                        </html>
                    ";

                    await _emailService.SendOrderConfirmationEmailAsync(user.Email, subject, htmlContent);

                    // Send notification using NotificationService
                    await _notification.SendNotificationAsync( user.Id,
                       $"Your payment for order {orderId} was successful. Total Amount: {order.TotalAmount:C}");
                }
            }

            return new PaymentResponse
            {
                OrderId = orderId,
                IsSuccess = isSuccess,
                StripePaymentIntentId = session.PaymentIntentId,
                CheckoutSessionId = session.Id,
                Status = session.PaymentStatus,
                Message = message
            };
        }

        public async Task<PaymentResponse?> ProcessWebhookAsync(string json, StringValues stripeSignature , string webhookSecretkey)
        {
            try
            {
                var webhookSecret = webhookSecretkey;
                if (string.IsNullOrEmpty(webhookSecret))
                    throw new Exception("Stripe webhook secret is not configured");

                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    stripeSignature,
                    webhookSecret
                );

                if (stripeEvent.Type == "checkout.session.completed")
                {
                    var session = stripeEvent.Data.Object as Session;

                    if (session != null && session.PaymentStatus == "paid")
                    {
                        if (Guid.TryParse(session.ClientReferenceId, out var orderId))
                        {
                            return new PaymentResponse
                            {
                                OrderId = orderId,
                                IsSuccess = true,
                                StripePaymentIntentId = session.PaymentIntentId,
                                CheckoutSessionId = session.Id,
                                Status = "paid",
                                Message = "Payment successfully processed"
                            };
                        }
                    }
                }
                else if (stripeEvent.Type == "payment_intent.succeeded")
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    return new PaymentResponse
                    {
                        OrderId = Guid.Empty,
                        IsSuccess = true,
                        StripePaymentIntentId = paymentIntent?.Id,
                        CheckoutSessionId = null,
                        Status = "succeeded",
                        Message = "Payment intent succeeded"
                    };
                }
                else if (stripeEvent.Type == "payment_intent.payment_failed")
                {
                    var failedPayment = stripeEvent.Data.Object as PaymentIntent;
                    return new PaymentResponse
                    {
                        OrderId = Guid.Empty,
                        IsSuccess = false,
                        StripePaymentIntentId = failedPayment?.Id,
                        CheckoutSessionId = null,
                        Status = "failed",
                        Message = "Payment failed"
                    };
                }

                return new PaymentResponse
                {
                    IsSuccess = false,
                    Status = "unhandled_event",
                    Message = $"Unhandled event type: {stripeEvent.Type}"
                };
            }
            catch (StripeException e)
            {
                return new PaymentResponse
                {
                    IsSuccess = false,
                    Message = $"Stripe error: {e.Message}"
                };
            }
            catch (Exception e)
            {
                return new PaymentResponse
                {
                    IsSuccess = false,
                    Message = $"Webhook processing error: {e.Message}"
                };
            }
        }
    }
}
