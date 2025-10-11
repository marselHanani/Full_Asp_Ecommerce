using System.IdentityModel.Tokens.Jwt;
using Ecommerce.Application.Dtos.Request;
using Ecommerce.Application.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecommerce.API.Area.Customer.Controllers
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Customer")]
    [Authorize(Roles = "Customer")]
    public class CheckoutController(CheckoutService service) : ControllerBase
    {
        private readonly CheckoutService _service = service;
        private Guid GetUserId()
        {
            var claim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ??
                        User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (claim == null)
                throw new UnauthorizedAccessException("User ID claim missing.");
            return Guid.Parse(claim);
        }
        [HttpPost("")]
        public async Task<IActionResult> ProcessCheckout([FromBody] CheckoutRequest request)
        {
            request.UserId = GetUserId().ToString();
            var result = await _service.ProcessCheckoutAsync(request, Request);
            return Ok(result);
        }
    }
}
