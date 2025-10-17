using Ecommerce.Application.Service;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace Ecommerce.API.Area.Customer.Controllers
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Customer")]
    [Authorize(Roles = "Customer")]

    public class NotificationsController(NotificationService service) : ControllerBase
    {
        private readonly NotificationService _service = service;

        [HttpGet]
        public async Task<IActionResult> GetUserNotifications()
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var notifications = await _service.GetUserNotificationsAsync(userId);
            return Ok(notifications);
        }
    }
}
