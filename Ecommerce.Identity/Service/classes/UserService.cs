using Ecommerce.Identity.Data;
using Ecommerce.Identity.Dtos.Request;
using Ecommerce.Identity.Dtos.Response;
using Ecommerce.Identity.Entities;
using Ecommerce.Identity.Service.interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
namespace Ecommerce.Identity.Service.classes
{
    public class UserService(
        AppIdentityDbContext db,
        IConfiguration config,
        IJwtTokenService jwtTokenService,
        UserManager<ApplicationUser> userManager,
        IEmailService emailService,
        SignInManager<ApplicationUser> signInManager)
        : IUserService
    {
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly AppIdentityDbContext _db = db;
        private readonly IConfiguration _config = config;
        private readonly IJwtTokenService _jwtTokenService = jwtTokenService;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IEmailService _emailService = emailService;

        public async Task<string> RegisterUserAsync(RegisterDto registerRequest, HttpRequest request)
        {
            var user = new ApplicationUser
            {
                Email = registerRequest.Email,
                UserName = registerRequest.Email.Split("@")[0],
                FullName = registerRequest.FullName,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, registerRequest.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join("|", result.Errors.Select(x => x.Description));
                throw new InvalidOperationException(errors);
            }

            await _userManager.AddToRoleAsync(user, "Customer");
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            await _emailService.SendEmailAsync(
                user.Email!,
                user.Id!,
                token,
                request
            );

            return user.Id!;
        }

        public async Task<LoginResponse> LoginUserAsync(LoginDto loginRequest)
        {
            var user = await _userManager.FindByEmailAsync(loginRequest.Email);
            if (user == null)
                throw new InvalidOperationException("Invalid email or password.");

            if (!user.IsActive)
                throw new UnauthorizedAccessException("Your account is blocked. Please contact support.");

            if (!user.EmailConfirmed)
                throw new UnauthorizedAccessException("Email not confirmed. Please confirm your email before logging in.");

            if (await _userManager.IsLockedOutAsync(user))
                throw new UnauthorizedAccessException("Your account is locked due to multiple failed login attempts. Please try again later.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginRequest.Password, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                    throw new UnauthorizedAccessException("Account locked due to multiple failed attempts. Try again later.");

                throw new InvalidOperationException("Invalid email or password.");
            }

            var (access, refresh) = await _jwtTokenService.GenerateTokensAsync(user);

            return new LoginResponse
            {
                Token = access,
                RefreshToken = refresh,
            };
        }

        public async Task<RefreshTokenResponse> RefreshTokenAsync(string refreshToken)
        {
            var hashed = JwtTokenService.HashToken(refreshToken);

            var tokenEntity = _db.RefreshTokens.FirstOrDefault(t => t.Token == hashed);

            if (tokenEntity == null || tokenEntity.IsRevoked || tokenEntity.ExpiresAt < DateTime.UtcNow)
                throw new InvalidOperationException("Invalid token");

            tokenEntity.IsRevoked = true;
            await _db.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(tokenEntity.UserId);
            if (user == null)
                throw new InvalidOperationException("Invalid user");

            var (access, refToken) = await _jwtTokenService.GenerateTokensAsync(user);

            return new RefreshTokenResponse
            {
                Token = access,
                RefreshToken = refToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60)
            };
        }

        public async Task<bool> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("Invalid user");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            return result.Succeeded;
        }
    }
}
