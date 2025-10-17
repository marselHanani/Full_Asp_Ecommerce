using Ecommerce.Application.Service;
using Ecommerce.Identity.Dtos.Request;
using Ecommerce.Identity.Service.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Ecommerce.API.Area.Customer.Controllers
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Customer")]
    [Authorize(Roles = "Customer")]

    public class ProfilesController(IUserProfileService service , OrderService orderService) : ControllerBase
    {
        private readonly IUserProfileService _service = service;
        private readonly OrderService _orderService = orderService;

        [HttpGet("me")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (userId == null) return Unauthorized();
            var profile = await _service.GetProfileAsync(userId);
            return Ok(profile);
        }
        [HttpPut("me")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileRequest request)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (userId == null) return Unauthorized();
            var result = await _service.UpdateProfileAsync(userId, request);
            if (!result) return BadRequest(new { Message = "Failed to update profile." });
            return NoContent();
        }
        [HttpPut("me/password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (userId == null) return Unauthorized();
            var result = await _service.ChangePasswordAsync(userId, request);
            if (!result) return BadRequest(new { Message = "Failed to change password." });
            return NoContent();
        }
        [HttpPost("me/profile-image")]
        public async Task<IActionResult> UpdateProfileImage(IFormFile imageFile)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (userId == null) return Unauthorized();
            var result = await _service.UpdateProfileImageAsync(userId, imageFile);
            if (!result) return BadRequest(new { Message = "Failed to update profile image." });
            return NoContent();
        }
        [HttpGet("me/orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (userId == null) return Unauthorized();
            var orders = await _orderService.GetOrdersByUserId(Guid.Parse(userId));
            return Ok(orders);
        }

    }
}
