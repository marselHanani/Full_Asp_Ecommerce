using Ecommerce.Identity.Service.classes;
using Ecommerce.Identity.Service.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Area.Admin.Controllers
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class ManageUsersController(IManageUserService service) : ControllerBase
    {
        private readonly IManageUserService _service = service;

        [HttpGet("users")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _service.GetAllUsers();
            return Ok(users);
        }

        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetById([FromRoute] string id)
        {
            var user = await _service.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPut("user/{id:guid}/block")]
        public async Task<IActionResult> BlockUser([FromRoute] Guid id)
        {
            var result = await _service.BlockUserAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
        [HttpPut("user/{id:guid}/unblock")]
        public async Task<IActionResult> UnblockUser([FromRoute] Guid id)
        {
            var result = await _service.UnblockUserAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("user/{id:guid}")]
        public async Task<IActionResult> DeleteUser([FromRoute] Guid id)
        {
            var result = await _service.DeleteUserAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPut("user/change-role/{id:guid}")]
        public async Task<IActionResult> ChangeUserRole([FromRoute] Guid id, [FromQuery] string newRole)
        {
            var result = await _service.ChangeUserRoleAsync(id, newRole);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

    }
}
