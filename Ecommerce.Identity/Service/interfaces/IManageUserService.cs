using Ecommerce.Identity.Dtos.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Identity.Service.interfaces
{
    public interface IManageUserService
    {
        Task<(ICollection<UserResponse> Users, int TotalCount)> GetAllUsers(
            string? search = null,
            int page = 1,
            int pageSize = 10,
            bool? isActive = null);
        Task<bool> ChangeUserRoleAsync(Guid userId, string newRole);
        Task<UserResponse> GetUserById(string userId);
        Task<bool> BlockUserAsync(Guid userId);
        Task<bool> UnblockUserAsync(Guid userId);
        Task<bool> DeleteUserAsync(Guid userId);


    }
}
