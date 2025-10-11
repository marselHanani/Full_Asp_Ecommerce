using Ecommerce.Application.Dtos.Request;
using Ecommerce.Application.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Area.Admin.Controllers
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Admin")]
    public class CategoriesController(CategoryService service) : ControllerBase
    {
        private readonly CategoryService _service = service;

        [HttpPost("")]
        public async Task<IActionResult> CreateNewCategory([FromBody]CategoryRequest request)
        {
            await _service.CreateNewCategory(request);
            return StatusCode(201);
        }
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateCategory([FromRoute] Guid id, [FromBody] CategoryRequest request)
        {
            await _service.UpdateCategory(id, request);
            return NoContent();
        }
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteCategory([FromRoute] Guid id)
        {
            await _service.DeleteCategory(id);
            return NoContent();
        }
    }
}
