using Ecommerce.Identity.Entities;

namespace Ecommerce.Identity.Service.interfaces;

public interface IJwtTokenService
{
    Task<string> GenerateAccessTokenAsync(ApplicationUser user);
    Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(ApplicationUser user);
}