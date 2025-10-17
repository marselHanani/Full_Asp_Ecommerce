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
        public async Task<IActionResult> GetAllCategories([FromQuery] string? search,
            Guid? filterByBrand, Guid? filterByCategory, [FromQuery] int pageSize = 10, [FromQuery] int page = 1)
        {
            var categories = await _service.GetAllCategories(search , page, pageSize, filterByBrand, filterByCategory);
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
        [HttpGet("{id:guid}/subCategories")]
        public async Task<IActionResult> GetSubCategoriesByCategoryId([FromRoute] Guid id)
        {
            var subCategories = await _service.GetSubCategoriesByCategoryId(id);
            return Ok(subCategories);
        }
        [HttpGet("parent")]
        public async Task<IActionResult> GetParentCategories()
        {
            var parentCategories = await _service.GetParentCategories();
            return Ok(parentCategories);
        }
        [HttpGet("subs")]
        public async Task<IActionResult> GetAllSubCategories()
        {
            var subCategories = await _service.GetAllSubCategories();
            return Ok(subCategories);
        }
    }
}
