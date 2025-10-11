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
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _service.GetAllProducts();
            return Ok(products);
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
