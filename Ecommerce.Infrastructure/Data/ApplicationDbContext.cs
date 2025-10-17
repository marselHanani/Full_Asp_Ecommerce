using Ecommerce.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Wishlist> Wishlists { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Category
        modelBuilder.Entity<Category>(b =>
        {
            b.ToTable("Categories");
            b.HasKey(c => c.Id);

            b.Property(c => c.Name).IsRequired().HasMaxLength(150);
            b.Property(c => c.Slug).IsRequired().HasMaxLength(160);

            b.HasOne(c => c.ParentCategory)
             .WithMany(c => c.SubCategories)
             .HasForeignKey(c => c.ParentCategoryId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // Brand
        modelBuilder.Entity<Brand>(b =>
        {
            b.ToTable("Brands");
            b.HasKey(bd => bd.Id);

            b.Property(bd => bd.Name).IsRequired().HasMaxLength(150);
            b.Property(bd => bd.Slug).IsRequired().HasMaxLength(160);
        });

        // Product
        modelBuilder.Entity<Product>(b =>
        {
            b.ToTable("Products");
            b.HasKey(p => p.Id);

            b.Property(p => p.Name).IsRequired().HasMaxLength(250);
            b.Property(p => p.Slug).IsRequired().HasMaxLength(260);
            b.Property(p => p.Description).HasMaxLength(4000);

            // Owned value object Money
            b.OwnsOne(p => p.Price, m =>
            {
                m.Property(x => x.Amount).HasColumnName("Price_Amount").IsRequired();
                m.Property(x => x.Currency).HasColumnName("Price_Currency").HasMaxLength(5).IsRequired();
            });

            b.Property(p => p.Stock).IsRequired();
            b.HasOne(p => p.Category).WithMany(c => c.Products).HasForeignKey(p => p.CategoryId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(p => p.Brand).WithMany(bd => bd.Products).HasForeignKey(p => p.BrandId).OnDelete(DeleteBehavior.Restrict);

            b.HasMany(p => p.Images).WithOne(i => i.Product).HasForeignKey(i => i.ProductId).OnDelete(DeleteBehavior.Cascade);
            b.HasMany(p => p.Reviews).WithOne(r => r.Product).HasForeignKey(r => r.ProductId).OnDelete(DeleteBehavior.Cascade);
        });

        // ProductImage
        modelBuilder.Entity<ProductImage>(b =>
        {
            b.ToTable("ProductImages");
            b.HasKey(pi => pi.Id);

            b.Property(pi => pi.FileName).IsRequired().HasMaxLength(1000);
            b.Property(pi => pi.IsMain).IsRequired();
        });

        // Review
        modelBuilder.Entity<Review>(b =>
        {
            b.ToTable("Reviews");
            b.HasKey(r => r.Id);

            b.Property(r => r.Rating).IsRequired();
            b.Property(r => r.Title).HasMaxLength(250);
            b.Property(r => r.Content).HasMaxLength(4000);
            b.Property(r => r.UserId).IsRequired();
        });
        modelBuilder.Entity<Review>().HasMany(r => r.Reviews)
        .WithOne(r => r.ParentReview)
        .HasForeignKey(r => r.ParentReviewId)
        .OnDelete(DeleteBehavior.NoAction); // Prevent cascade delete cycles
        modelBuilder.Owned<Money>();
        // Order
        modelBuilder.Entity<Order>(b =>
        {
            b.HasKey(o => o.Id);
            b.HasMany(o => o.Items)
                .WithOne(i => i.Order)
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // OrderItem
        modelBuilder.Entity<OrderItem>(b =>
        {
            b.HasKey(oi => oi.Id);
        });

        // Payment
        modelBuilder.Entity<Payment>(b =>
        {
            b.HasKey(p => p.Id);
            b.HasOne(p => p.Order)
                .WithMany()
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Indexes (for performance)
        modelBuilder.Entity<Product>().HasIndex(p => p.Slug).IsUnique(false);
        modelBuilder.Entity<Category>().HasIndex(c => c.Slug).IsUnique(false);
        modelBuilder.Entity<Brand>().HasIndex(b => b.Slug).IsUnique(false);
    }
}