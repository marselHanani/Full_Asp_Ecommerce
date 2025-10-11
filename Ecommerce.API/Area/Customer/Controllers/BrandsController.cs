using Ecommerce.Application.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Area.Customer.Controllers
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Customer")]
    public class BrandsController(BrandService service) : ControllerBase
    {
        private readonly BrandService _service = service;

        [HttpGet("")]
        public async Task<IActionResult> GetAllBrands()
        {
            var brands =await _service.GetAllBrands();
            return Ok(brands);
        }
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetBrandById([FromRoute] Guid id)
        {
            var brand = await _service.GetBrandById(id);
            if (brand is null) return NotFound();
            return Ok(brand);
        }
    }
}
