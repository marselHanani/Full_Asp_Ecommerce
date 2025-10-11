namespace Ecommerce.Identity.Dtos.Response;

public class LoginResponse
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
}