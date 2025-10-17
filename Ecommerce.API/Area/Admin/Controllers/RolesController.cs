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

    public class RolesController(IRoleService service) : ControllerBase
    {
        private readonly IRoleService _service = service;

        [HttpGet("roles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _service.GetAllRolesAsync();
            return Ok(roles);
        }

        [HttpGet("role/{roleName}")]
        public async Task<IActionResult> GetRole([FromRoute] string roleName)
        {
            var role = await _service.GetRoleAsync(roleName);
            if (role == null)
            {
                return NotFound("Role not found.");
            }
            return Ok(role);
        }


        [HttpPost("role")]
        public async Task<IActionResult> CreateRole([FromBody] string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                return BadRequest("Role name cannot be empty.");
            }
            var exists = await _service.RoleExistsAsync(roleName);
            if (exists)
            {
                return Conflict("Role already exists.");
            }
            var result = await _service.CreateRoleAsync(roleName);
            if (result)
            {
                return CreatedAtAction(nameof(GetAllRoles), new { roleName }, roleName);
            }
            return StatusCode(500, "An error occurred while creating the role.");
        }

        [HttpPut("role/{roleName}")]
        public async Task<IActionResult> UpdateRole([FromRoute] string roleName, [FromBody] string newRoleName)
        {
            if (string.IsNullOrWhiteSpace(newRoleName))
            {
                return BadRequest("New role name cannot be empty.");
            }
            var exists = await _service.RoleExistsAsync(roleName);
            if (!exists)
            {
                return NotFound("Role not found.");
            }
            var updatedRole = await _service.UpdateRoleAsync(roleName, newRoleName);
            if (updatedRole != null)
            {
                return Ok(updatedRole);
            }
            return StatusCode(500, "An error occurred while updating the role.");
        }

        [HttpDelete("role/{roleName}")]
        public async Task<IActionResult> DeleteRole([FromRoute] string roleName)
        {
            var exists = await _service.RoleExistsAsync(roleName);
            if (!exists)
            {
                return NotFound("Role not found.");
            }
            var result = await _service.DeleteRoleAsync(roleName);
            if (result)
            {
                return NoContent();
            }
            return StatusCode(500, "An error occurred while deleting the role.");
        }

    }
}
