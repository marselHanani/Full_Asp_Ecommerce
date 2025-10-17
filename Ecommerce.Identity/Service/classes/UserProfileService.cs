using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Identity.Dtos.Request;
using Ecommerce.Identity.Dtos.Response;
using Ecommerce.Identity.Entities;
using Ecommerce.Identity.Service.interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Identity.Service.classes
{
    public class UserProfileService(IHttpContextAccessor http, UserManager<ApplicationUser> userManager ) : IUserProfileService
    {
        private readonly IHttpContextAccessor _http = http;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<UserProfileResponse> GetProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new Exception("User not found");

            var profileImageUrl = string.IsNullOrEmpty(user.ProfileImageUrl)
                ? null
                : $"{_http.HttpContext.Request.Scheme}://{_http.HttpContext.Request.Host}{user.ProfileImageUrl}";

            return new UserProfileResponse
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                ProfileImageUrl = profileImageUrl,
                Address = user.Address,
                City = user.City,
                Country = user.Country,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<bool> UpdateProfileAsync(string userId, UpdateUserProfileRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new Exception("User not found");

            user.FullName = request.FullName;
            user.Address = request.Address;
            user.City = request.City;
            user.Country = request.Country;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> ChangePasswordAsync(string userId, ChangePasswordRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new Exception("User not found");

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            return result.Succeeded;
        }

        public async Task<bool> UpdateProfileImageAsync(string userId, IFormFile imageFile)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new Exception("User not found");

            // Check if user already has a profile image and remove it
            if (!string.IsNullOrEmpty(user.ProfileImageUrl))
            {
                var existingImagePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    user.ProfileImageUrl.TrimStart('/'));

                if (File.Exists(existingImagePath))
                {
                    File.Delete(existingImagePath);
                }
            }

            // Create and save the new image
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
            var directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/profile_images");

            // Ensure directory exists
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var path = Path.Combine(directory, fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            user.ProfileImageUrl = $"/profile_images/{fileName}";
            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded;
        }
    }
}
