using System.IdentityModel.Tokens.Jwt;
using Ecommerce.Application.Dtos.Request;
using Ecommerce.Application.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Ecommerce.API.Area.Customer.Controllers
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Customer")]
    public class CartsController(CartService service) : ControllerBase
    {
        private readonly CartService _service = service;

        private Guid GetUserId()
        {
            var claim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ??
                        User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (claim == null)
                throw new UnauthorizedAccessException("User ID claim missing.");
            return Guid.Parse(claim);
        }

        [HttpGet("GetCart")]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetUserId();
            var cart = await _service.GetCartByUserIdAsync(userId.ToString());
            if (cart == null)
            {
                return NotFound();
            }
            return Ok(cart);
        }
        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest cartItem)
        {
            var userId = GetUserId();
            var result = await _service.AddItemToCartAsync(cartItem, userId.ToString());
            if (result == null)
            {
                return BadRequest("Could not add item to cart.");
            }
            return Ok(new {message ="Item added to cart successfully.",result});
        }
        [HttpDelete("ClearCart")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetUserId();
            await _service.ClearCartAsync(userId.ToString());
            return Ok(new { message = "Cart cleared successfully." });
        }
        [HttpDelete("cart-item/")]
        public async Task<IActionResult> RemoveCartItem(RemoveCartItemRequest request)
        {
            var userId = GetUserId();
            request.UserId = userId.ToString();
            await _service.RemoveCartItemAsync(request);
            return Ok(new { message = "Cart item removed successfully." });
        }
    }
}
