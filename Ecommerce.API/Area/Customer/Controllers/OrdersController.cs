using Azure.Core;
using Ecommerce.Application.Dtos.Request;
using Ecommerce.Application.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe.Forwarding;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Ecommerce.API.Area.Customer.Controllers
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Customer")]
    public class OrdersController(OrderService service) : ControllerBase
    {
        private readonly OrderService _service = service;
        private Guid GetUserId()
        {
            var claim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ??
                        User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (claim == null)
                throw new UnauthorizedAccessException("User ID claim missing.");
            return Guid.Parse(claim);
        } 

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrdersByUserId()
        {
            var userId = GetUserId();
            var orders = await _service.GetOrdersByUserId(userId.ToString());
            if (!orders.Any())
            {
                return NotFound();
            }

            return Ok(orders);
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequest request)
        {
            request.UserId = GetUserId().ToString();
            var result =await _service.AddNewOrder(request, Request);
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateOrder(Guid id, [FromBody] OrderRequest request)
        {
            request.UserId = GetUserId().ToString();
            await _service.UpdateOrder(id, request);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            await _service.DeleteOrder(id);
            return NoContent();
        }
    }
}
