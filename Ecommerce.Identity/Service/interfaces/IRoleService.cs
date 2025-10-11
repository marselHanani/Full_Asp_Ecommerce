using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Identity.Dtos.Response;

namespace Ecommerce.Identity.Service.interfaces
{
    public interface IRoleService
    {
        Task<ICollection<RoleResponse>> GetAllRolesAsync();
        Task<bool> CreateRoleAsync(string roleName);
        Task<bool> DeleteRoleAsync(string roleName);
        Task<bool> RoleExistsAsync(string roleName);
        Task<RoleResponse> GetRoleAsync(string roleName);
        Task<RoleResponse?> UpdateRoleAsync(string roleName, string newRoleName);

    }
}
