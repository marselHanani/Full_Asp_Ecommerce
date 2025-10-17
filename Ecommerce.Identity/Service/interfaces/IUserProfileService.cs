using Ecommerce.Identity.Dtos.Request;
using Ecommerce.Identity.Dtos.Response;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Identity.Service.interfaces
{
    public interface IUserProfileService
    {
        Task<UserProfileResponse> GetProfileAsync(string userId);
        Task<bool> UpdateProfileAsync(string userId, UpdateUserProfileRequest request);
        Task<bool> ChangePasswordAsync(string userId, ChangePasswordRequest request);
        Task<bool> UpdateProfileImageAsync(string userId, IFormFile imageFile);
    }
}
