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
    public class OrdersController(OrderService service) : ControllerBase
    {
        private readonly OrderService _service = service;

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _service.GetAllOrders();
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
