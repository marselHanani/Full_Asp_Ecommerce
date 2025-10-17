using Ecommerce.Application.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Area.Admin.Controllers
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class DashboardAnalyticsController(AdminAnalyticsService service) : ControllerBase
    {
        private readonly AdminAnalyticsService _service = service;

        [HttpGet("summary")]
        public async Task<IActionResult> GetDashboardSummaryAsync()
        {
            var summary = await _service.GetDashboardDataAsync();
            return Ok(summary);
        } 
    }
}
