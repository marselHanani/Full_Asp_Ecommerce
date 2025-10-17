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
    [Authorize(Roles = "Admin")]

    public class OrdersController(OrderService service) : ControllerBase
    {
        private readonly OrderService _service = service;

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllOrders(
           [FromQuery] string? search = null,
           [FromQuery] string? status = null,
           [FromQuery] int page = 1,
           [FromQuery] int pageSize = 10)
        {
            var orders = await _service.GetAllOrders(search, status, page, pageSize);
            return Ok(orders);
        }
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetOrdersById(Guid id)
        {
            var order = await _service.GetOrderById(id);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }
    }
}
