using Ecommerce.Identity.Dtos.Request;
using Ecommerce.Identity.Dtos.Response;
using Microsoft.AspNetCore.Http;

namespace Ecommerce.Identity.Service.interfaces;

public interface IUserService
{
    Task<string> RegisterUserAsync(RegisterDto registerRequest, HttpRequest request);
    Task<LoginResponse> LoginUserAsync(LoginDto loginRequest);
    Task<RefreshTokenResponse> RefreshTokenAsync(string refreshToken);
    Task<bool> ConfirmEmailAsync(string userId, string token);
}