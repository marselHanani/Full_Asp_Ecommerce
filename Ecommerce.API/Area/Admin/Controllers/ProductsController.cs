using Ecommerce.Application.Dtos.Request;
using Ecommerce.Application.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Area.Admin.Controllers
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Admin")]
    public class ProductsController(ProductService service) : ControllerBase
    {
        private readonly ProductService _service = service;

        [HttpPost("")]
        public async Task<IActionResult> CreateNewProduct([FromForm] ProductRequest request)
        {
            await _service.CreateNewProduct(request);
            return StatusCode(201);
        }
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateProduct([FromRoute] Guid id, [FromForm] ProductRequest request)
        {
            await _service.UpdateProduct(id, request);
            return NoContent();
        }
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] Guid id)
        {
            await _service.DeleteProduct(id);
            return NoContent();
        }
    }
}
