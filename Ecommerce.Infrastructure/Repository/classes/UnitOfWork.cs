using Ecommerce.Application.Dtos.Request;
using Ecommerce.Application.Dtos.Response;
using Ecommerce.Domain.Entity;
using Ecommerce.Domain.Repository.Interfaces;
using Ecommerce.Infrastructure.Data;

namespace Ecommerce.Infrastructure.Repository.classes
{
    public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
    {
        private readonly ApplicationDbContext _context = context;

        public void Dispose() => _context.Dispose();


        public IGenericRepository<Product> Products { get; private set; } = new GenericRepository<Product>(context);
        public IGenericRepository<Category> Categories { get; } = new GenericRepository<Category>(context);
        public IGenericRepository<Brand> Brands { get; } = new GenericRepository<Brand>(context);
        public IGenericRepository<ProductImage> ProductImages { get; } = new GenericRepository<ProductImage>(context);
        public IGenericRepository<Review> Reviews { get; } = new GenericRepository<Review>(context);
        public IGenericRepository<Order> Orders { get; } = new GenericRepository<Order>(context);
        public IGenericRepository<OrderItem> OrderItems { get; } = new GenericRepository<OrderItem>(context);
        public IGenericRepository<Payment> Payments { get; } = new GenericRepository<Payment>(context);
        public IGenericRepository<Cart> Carts { get; } = new GenericRepository<Cart>(context);
        public IGenericRepository<CartItem> CartItems { get; } = new GenericRepository<CartItem>(context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
