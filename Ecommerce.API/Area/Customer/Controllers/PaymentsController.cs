using Ecommerce.Application.Dtos.Request;
using Ecommerce.Application.Dtos.Response;
using Ecommerce.Application.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Threading.Tasks;
using Stripe.BillingPortal;

namespace Ecommerce.API.Area.Customer.Controllers
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Customer")]
    public class PaymentsController(PaymentService service , IConfiguration config) : ControllerBase
    {
        private readonly PaymentService _service = service;
        private readonly IConfiguration _config = config;


        [HttpPost("create-checkout-session")]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] PaymentRequest request)
        {
            var result = await _service.CreateCheckoutSessionAsync(request, Request);
            return Ok(result);
        }

        [HttpGet("session/{sessionId}")]
        public async Task<IActionResult> ChangePaymentStatus([FromRoute] string sessionId)
        {
            var result = await _service.GetCheckoutSessionStatusAsync(sessionId);
            return Ok(result);
        }

        [HttpGet("checkout/success")]
        public async Task<IActionResult> CheckoutSuccess([FromQuery] string session_id, [FromQuery] Guid order_id)
        {
            if (string.IsNullOrEmpty(session_id))
            {
                return BadRequest("Session ID is required");
            }

            var result = await _service.GetCheckoutSessionStatusAsync(session_id);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            // Additional processing specific to successful checkout can be added here

            return Ok(result);
        }

        [HttpGet("checkout/cancel")]
        public async Task<IActionResult> CheckoutCancel([FromQuery] Guid order_id)
        {
            if (order_id == Guid.Empty)
            {
                return BadRequest("Order ID is required");
            }

            // Here you would typically:
            // 1. Update the order status to cancelled
            // 2. Release any reserved inventory
            // 3. Log the cancellation

            return Ok(new
            {
                IsSuccess = true,
                Message = "Payment process was cancelled",
                OrderId = order_id
            });
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> HandleWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            if (!Request.Headers.TryGetValue("Stripe-Signature", out var stripeSignature))
            {
                return BadRequest(new { message = "Stripe-Signature header is missing" });
            }

            var webhookSecret = _config["Stripe:WebhookSecret"];

            if (string.IsNullOrEmpty(webhookSecret))
            {
                return BadRequest(new { message = "Stripe webhook secret is not configured" });
            }

            try
            {
                // Pass the correct arguments to ProcessWebhookAsync
                var result = await _service.ProcessWebhookAsync(json, stripeSignature, webhookSecret);

                if (result != null && result.IsSuccess)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (StripeException ex)
            {
                return BadRequest(new { message = $"Stripe error: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

    }
}
