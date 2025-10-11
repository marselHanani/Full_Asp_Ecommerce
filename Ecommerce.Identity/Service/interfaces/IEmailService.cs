using Microsoft.AspNetCore.Http;

namespace Ecommerce.Identity.Service.interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string userId, string token, HttpRequest request, string? bodyContent = null);
    Task SendOrderConfirmationEmailAsync(string to, string subject, string htmlContent);
}