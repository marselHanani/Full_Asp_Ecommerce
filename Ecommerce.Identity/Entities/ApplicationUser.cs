using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Identity.Entities;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public string? ProfilePicture { get; set; }
    public bool IsSeller { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}