using Ecommerce.Application.Dtos.Response;
using Ecommerce.Application.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Area.Customer.Controllers
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Customer")]
    public class ProductsController(ProductService service) : ControllerBase
    {
        private readonly ProductService _service = service;

        [HttpGet("")]
        public async Task<IActionResult> GetAllProducts(
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] Guid? categoryId = null,
            [FromQuery] Guid? brandId = null)
        {
            var (products, totalCount) = await _service.GetAllProducts(
                search, page, pageSize, minPrice, maxPrice, categoryId, brandId
            );

            return Ok(new
            {
                Products = products,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetProductById([FromRoute] Guid id)
        {
            var product = await _service.GetProductById(id);
            if (product is null) return NotFound();
            return Ok(product);
        }
        [HttpGet("category/{slug}")]
        public async Task<IActionResult> GetProductByCategorySlug([FromRoute] string slug)
        {
            var product = await _service.GetProductByCategorySlug(slug);
            if (product is null) return NotFound();
            return Ok(product);
        }
        [HttpGet("brand/{slug}")]
        public async Task<IActionResult> GetProductByBrandSlug([FromRoute] string slug)
        {
            var product = await _service.GetProductByBrandSlug(slug);
            if (product is null) return NotFound();
            return Ok(product);
        }

    }
}
