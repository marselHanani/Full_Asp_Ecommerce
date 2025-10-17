using Ecommerce.Domain.Entity;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Identity.Entities;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public string? ProfileImageUrl { get; set; }
    public bool IsSeller { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }

    public ICollection<Order>? Orders { get; set; }
}