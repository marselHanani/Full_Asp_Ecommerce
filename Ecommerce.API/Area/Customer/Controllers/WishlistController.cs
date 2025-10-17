using Ecommerce.Application.Dtos.Request;
using Ecommerce.Application.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Ecommerce.API.Area.Customer.Controllers
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Customer")]
    [Authorize(Roles = "Customer")]
    public class WishlistController(WishlistService service) : ControllerBase
    {
        private readonly WishlistService _service = service;

        private string GetUserId()
        {
            var claim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ??
                        User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (claim == null)
                throw new UnauthorizedAccessException("User ID claim missing.");
            return claim;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToWishlist([FromBody] AddWishlistRequest request)
        {
            await _service.AddToWishlist(GetUserId(), request.ProductId);
            return Ok(new { message = "Added to wishlist" });
        }

        [HttpDelete("remove/{productId:guid}")]
        public async Task<IActionResult> RemoveFromWishlist(Guid productId)
        {
            await _service.RemoveFromWishlist(GetUserId(), productId);
            return Ok(new { message = "Removed from wishlist" });
        }

        [HttpGet("")]
        public async Task<IActionResult> GetWishlist()
        {
            var wishlist = await _service.GetWishlist(GetUserId());
            return Ok(wishlist);
        }
    }
}
