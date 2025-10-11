using Ecommerce.Common;

namespace Ecommerce.Identity.Entities;

public class RefreshToken: BaseEntity
{
    public string Token { get; set; }
    public string UserId { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}