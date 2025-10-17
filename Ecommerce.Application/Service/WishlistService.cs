// Ecommerce.Application\Service\WishlistService.cs
using Ecommerce.Domain.Entity;
using Ecommerce.Domain.Repository.Interfaces;

namespace Ecommerce.Application.Service
{
    public class WishlistService(IUnitOfWork unit, NotificationService notification,CacheService cache)
    {
        private readonly IUnitOfWork _unit = unit;
        private readonly NotificationService _notification = notification;
        private readonly CacheService _cache = cache;

        public async Task AddToWishlist(string userId, Guid productId)
        {
            var wishlist = new Wishlist
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ProductId = productId,
                DateAdded = DateTime.UtcNow
            };
            await _unit.Wishlists.AddAsync(wishlist);
            await _unit.SaveChangesAsync();
            await _notification.SendNotificationAsync(userId, $"Product {productId} added to wishlist.");
        }

        public async Task RemoveFromWishlist(string userId, Guid productId)
        {
            var repo = _unit.Wishlists;
            var item = (await repo.GetAllAsync())
                .FirstOrDefault(w => w.UserId == userId && w.ProductId == productId);
            if (item != null)
            {
                repo.Delete(item.Id);
                await _unit.SaveChangesAsync();
                await _notification.SendNotificationAsync(userId, $"Product {productId} removed from wishlist.");
            }
        }

        public async Task<IEnumerable<Wishlist>> GetWishlist(string userId)
        {
            var cacheKey = $"wishlist_{userId}";
            var cachedWishlist = await _cache.GetAsync<IEnumerable<Wishlist>>(cacheKey);
            if (cachedWishlist != null)
            {
                return cachedWishlist;
            }

            var repo = _unit.Wishlists;
            var wishlist = (await repo.GetAllAsync()).Where(w => w.UserId == userId).ToList();
            await _cache.SetAsync(cacheKey, wishlist, TimeSpan.FromMinutes(10));
            return wishlist;
        }
    }
}