using Ecommerce.Domain.Entity;
using Ecommerce.Identity.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Ecommerce.Identity.Data;

public class AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<ApplicationUser>().ToTable("Users");
        builder.Owned<Money>();
        builder.Entity<RefreshToken>(b =>
        {
            b.HasKey(r => r.Id);
            b.Property(r => r.Token).IsRequired();
            b.HasIndex(r => r.Token).IsUnique();
            b.Property(r => r.UserId).IsRequired();
            b.HasOne<ApplicationUser>().WithMany().HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}