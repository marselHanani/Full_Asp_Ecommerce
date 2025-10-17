using Ecommerce.Application.Dtos.Request;
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
    public class BrandsController(BrandService service) : ControllerBase
    {
        private readonly BrandService _service = service;

        [HttpPost("")]
        public async Task<IActionResult> CreateNewBrand([FromForm] BrandRequest request)
        {
            await _service.CreateNewBrand(request);
            return StatusCode(201);
        }
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateBrand([FromRoute] Guid id, [FromForm] BrandRequest request)
        {
            await _service.UpdateBrand(id, request);
            return NoContent();
        }
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteBrand([FromRoute] Guid id)
        {
            await _service.DeleteBrand(id);
            return NoContent();
        }
    }
}
