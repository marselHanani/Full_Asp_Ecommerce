using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Identity.Dtos.Response;
using Ecommerce.Identity.Service.interfaces;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Identity.Service.classes
{
    public class RoleService(RoleManager<IdentityRole> roleManager) : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;

        public async Task<ICollection<RoleResponse>> GetAllRolesAsync()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return roles.Adapt<ICollection<RoleResponse>>();
        }

        public async Task<bool> CreateRoleAsync(string roleName)
        {
            var role = new IdentityRole(roleName);
            var result = await _roleManager.CreateAsync(role);
            return result.Succeeded;
        }

        public async Task<bool> DeleteRoleAsync(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null) return false;

            var result = await _roleManager.DeleteAsync(role);
            return result.Succeeded;
        }
        public async Task<bool> RoleExistsAsync(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            return role != null;
        }

        public async Task<RoleResponse> GetRoleAsync(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null) return null;
            return role.Adapt<RoleResponse>();
        }

        public async Task<RoleResponse?> UpdateRoleAsync(string roleName, string newRoleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null) return null;

            role.Name = newRoleName;
            var result = await _roleManager.UpdateAsync(role);
            return result.Succeeded ? role.Adapt<RoleResponse>() : null;
        }
    }
}
