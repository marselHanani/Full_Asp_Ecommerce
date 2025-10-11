using Ecommerce.Application.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Area.Customer.Controllers
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Customer")]
    public class CategoriesController(CategoryService service) : ControllerBase
    {
        private readonly CategoryService _service = service;

        [HttpGet("")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _service.GetAllCategories();
            return Ok(categories);
        }
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetCategoryById([FromRoute] Guid id)
        {
            var category = await _service.GetCategoryById(id);
            if (category is null) return NotFound();
            return Ok(category);
        }
        [HttpGet("slug/{slug}")]
        public async Task<IActionResult> GetCategoryBySlug([FromRoute] string slug)
        {
            var category = await _service.GetCategoryBySlug(slug);
            if (category is null) return NotFound();
            return Ok(category);
        }
    }
}
