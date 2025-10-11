using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Common;

namespace Ecommerce.Domain.Entity
{
    public class Money
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";

        // Parameterless constructor for EF
        public Money() { }

        public Money(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }
    }
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string Slug { get; set; }
        public string? Description { get; set; }
        public Money Price { get; set; }
        public int Stock { get; set; }
        public Guid CategoryId { get; set; }
        public Guid BrandId { get; set; }

        // Navigation
        public Category Category { get; private set; } = null!;
        public Brand Brand { get; private set; } = null!;
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public ICollection<Review> Reviews { get; private set; } = new List<Review>();

        public void UpdateStock(int delta)
        {
            var newStock = Stock + delta;
            if (newStock < 0) throw new InvalidOperationException("Stock cannot be negative");
            Stock = newStock;
        }
    }
}
