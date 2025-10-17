using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Identity.Dtos.Response;
using Ecommerce.Identity.Entities;
using Ecommerce.Identity.Service.interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Identity.Service.classes
{
    public class ManageUserService(UserManager<ApplicationUser> manager) : IManageUserService
    {
        private readonly UserManager<ApplicationUser> _manager = manager;

        public async Task<(ICollection<UserResponse> Users, int TotalCount)> GetAllUsers(
            string? search = null,
            int page = 1,
            int pageSize = 10,
            bool? isActive = null)
        {
            var query = _manager.Users.AsQueryable();

            // Filter by active status if provided
            if (isActive.HasValue)
                query = query.Where(u => u.IsActive == isActive.Value);

            // Search by username or email if provided
            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.Trim().ToLowerInvariant();
                query = query.Where(u =>
                    u.UserName.ToLower().Contains(searchLower) ||
                    u.Email.ToLower().Contains(searchLower));
            }

            var totalCount = await query.CountAsync();

            // Pagination
            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new List<UserResponse>();
            foreach (var user in users)
            {
                var roles = await _manager.GetRolesAsync(user);
                result.Add(new UserResponse
                {
                    Id = Guid.Parse(user.Id),
                    UserName = user.UserName!,
                    Email = user.Email!,
                    PhoneNumber = user.PhoneNumber,
                    EmailConfirmed = user.EmailConfirmed,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    Roles = roles.ToList()
                });
            }

            return (result, totalCount);
        }

        public async Task<UserResponse> GetUserById(string userId)
        {
            var user = await _manager.FindByIdAsync(userId);
            if (user == null)
                return null;

            var roles = await _manager.GetRolesAsync(user);

            return new UserResponse
            {
                Id = Guid.Parse(user.Id),
                UserName = user.UserName!,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber,
                EmailConfirmed = user.EmailConfirmed,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                Roles = roles.ToList()
            };
        }

        public async Task<bool> BlockUserAsync(Guid userId)
        {
            var user = await _manager.FindByIdAsync(userId.ToString());
            if (user == null)
                return false;

            if (!user.IsActive)
                return true; // already blocked

            user.IsActive = false;
            var result = await _manager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> UnblockUserAsync(Guid userId)
        {
            var user = await _manager.FindByIdAsync(userId.ToString());
            if (user == null)
                return false;

            if (user.IsActive)
                return true; // already active

            user.IsActive = true;
            var result = await _manager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            var user = await _manager.FindByIdAsync(userId.ToString());
            if (user == null)
                return false;

            var result = await _manager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> ChangeUserRoleAsync(Guid userId, string newRole)
        {
            var user = await _manager.FindByIdAsync(userId.ToString());
            if (user == null)
                return false;

            var currentRoles = await _manager.GetRolesAsync(user);
            var removeResult = await _manager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
                return false;

            var addResult = await _manager.AddToRoleAsync(user, newRole);
            return addResult.Succeeded;
        }
    }
}
