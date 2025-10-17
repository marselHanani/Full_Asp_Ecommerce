using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Ecommerce.Application.Dtos.Request;
using Ecommerce.Application.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Area.Customer.Controllers
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Customer")]
    [Authorize(Roles = "Customer")]

    public class ReviewsController(ReviewService service) : ControllerBase
    {
        private readonly ReviewService _service = service;

        // Helper: get userId from JWT
        private Guid GetUserId()
        {
            var claim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ??
                        User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (claim == null)
                throw new UnauthorizedAccessException("User ID claim missing.");
            return Guid.Parse(claim);
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetAllReviewsByProductId(Guid productId)
        {
            var reviews = await _service.GetAllReviewsByProductId(productId);
            return Ok(reviews);
        }

        [HttpGet("user")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> GetAllReviewsByUserId()
        {
            var userId = GetUserId();

            // Customers can only access their own reviews
            if (User.IsInRole("Customer") && userId != GetUserId())
            {
                return Forbid();
            }

            var reviews = await _service.GetAllReviewsByUserId(userId);
            return Ok(reviews);
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllReviews()
        {
            var reviews = await _service.GetAllReviews();
            return Ok(reviews);
        }

        [HttpPost("")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> AddReview([FromBody] ReviewRequest request)
        {
            Guid userId;
            try
            {
                userId = GetUserId();
            }
            catch
            {
                return Unauthorized(new { Message = "User ID not found in token." });
            }

            await _service.AddReview(request, userId);
            return Ok(new { Message = "Review added successfully" });
        }

        [HttpPost("reply")]
        public async Task<IActionResult> ReplyOnReview([FromBody] ReplyReviewRequest request)
        {
            var userId = GetUserId();
            await _service.AddReplyToReview(request, userId);
            return Ok(new { Message = "Reply Add Successfully" });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReviewById(Guid id)
        {
            var review = await _service.GetReviewById(id);
            if (review == null)
                return NotFound(new { Message = "Review not found" });

            return Ok(review);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> UpdateReview(Guid id, [FromBody] ReviewRequest request)
        {
            var existingReview = await _service.GetReviewById(id);
            if (existingReview == null)
                return NotFound(new { Message = "Review not found" });

            await _service.UpdateReview(id, request);
            return Ok(new { Message = "Review updated successfully" });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> DeleteReview(Guid id)
        {
            var existingReview = await _service.GetReviewById(id);
            if (existingReview == null)
                return NotFound(new { Message = "Review not found" });

            await _service.DeleteReview(id);
            return Ok(new { Message = "Review deleted successfully" });
        }
    }
}
