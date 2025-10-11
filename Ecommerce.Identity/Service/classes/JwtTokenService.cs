using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Ecommerce.Identity.Data;
using Ecommerce.Identity.Entities;
using Ecommerce.Identity.Service.interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Ecommerce.Identity.Service.classes;

public class JwtTokenService(AppIdentityDbContext db, IConfiguration config, UserManager<ApplicationUser> userManager)
    : IJwtTokenService
{
    public async Task<string> GenerateAccessTokenAsync(ApplicationUser user)
    {
        var jwtSection = config.GetSection("jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["key"] ?? string.Empty));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var roles = await userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim("username", user.UserName ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var token = new JwtSecurityToken(
            issuer: jwtSection["issuer"],
            audience: jwtSection["audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSection["DurationInMinutes"])),
            signingCredentials: credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(ApplicationUser user)
    {
        var jwtSection = config.GetSection("jwt");
        var accessToken = await GenerateAccessTokenAsync(user);

        // Generate a secure refresh token (URL-safe)
        string refreshToken = GenerateSecureToken(64);

        // Hash the token and save the hash in DB
        var hashedToken = HashToken(refreshToken);

        var refreshEntity = new RefreshToken
        {
            Token = hashedToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(Convert.ToDouble(jwtSection["RefreshTokenDurationInDays"])),
            IsRevoked = false,
            CreatedAtUtc = DateTime.UtcNow
        };

        await db.AddAsync(refreshEntity);
        await db.SaveChangesAsync();

        return (accessToken, refreshToken); // return plain refreshToken to client
    }

    // ✅ Generate random refresh token in URL-safe format
    private static string GenerateSecureToken(int length)
    {
        var randomBytes = new byte[length];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Base64UrlEncode(randomBytes);
    }

    // ✅ Convert bytes to Base64Url (without + / =)
    private static string Base64UrlEncode(byte[] bytes)
    {
        var s = Convert.ToBase64String(bytes);
        s = s.TrimEnd('=');
        s = s.Replace('+', '-').Replace('/', '_');
        return s;
    }

    // ✅ Hash refresh token before storing
    public static string HashToken(string token)
    {
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hash);
    }
}