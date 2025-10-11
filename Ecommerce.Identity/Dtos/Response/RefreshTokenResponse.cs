namespace Ecommerce.Identity.Dtos.Response;

public class RefreshTokenResponse : LoginResponse
{
    public DateTime ExpiresAt { get; set; }
}