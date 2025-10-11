using Ecommerce.Application.Dtos.Request;
using Ecommerce.Identity.Dtos.Request;
using Ecommerce.Identity.Service.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Area.Customer.Controllers
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Customer")]
    public class AuthController(IUserService service) : ControllerBase
    {
        [HttpPost("Register")]
        public async Task<IActionResult>Register([FromBody] RegisterDto request) => Ok(await service.RegisterUserAsync(request, Request));
        
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request) => Ok(await service.LoginUserAsync(request));
        
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromQuery] string refreshToken) => Ok(await service.RefreshTokenAsync(refreshToken));
        
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token) => Ok(await service.ConfirmEmailAsync(userId, token));
    }
}
