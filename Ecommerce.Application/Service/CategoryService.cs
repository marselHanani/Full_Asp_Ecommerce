using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Application.Dtos.Request;
using Ecommerce.Application.Dtos.Response;
using Ecommerce.Domain.Entity;
using Ecommerce.Domain.Repository.Interfaces;
using Mapster;
using Slugify;

namespace Ecommerce.Application.Service
{
    public class CategoryService(IUnitOfWork unit,SlugHelper slugHelper,ICategoryRepository categoryRepo, CacheService cache)
    {
        private readonly IUnitOfWork _unit = unit;
        private readonly SlugHelper _slugHelper = slugHelper;
        private readonly ICategoryRepository _categoryRepo = categoryRepo;
        private readonly CacheService _cache = cache;

        public async Task<(List<CategoryResponse> categories, int TotalCount)> GetAllCategories(
            string? search, 
            int page = 1, 
            int pageSize = 10, 
            Guid? brandId = null, 
            Guid? categoryId = null)
        {
            var cacheKey = "Categories_all";
            var cached = await _cache.GetAsync<List<CategoryResponse>>(cacheKey);
            if (cached is not null) return (cached,cached.Count);

            var categories = await _categoryRepo.GetAllCategoriesWithProducts();

            // Filter by categoryId if provided
            if (categoryId.HasValue)
            {
                categories = categories
                    .Where(c => c.Id == categoryId.Value || (c.SubCategories != null && c.SubCategories.Any(sc => sc.Id == categoryId.Value)))
                    .ToList();
            }

            // Apply search if provided
            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.Trim().ToLowerInvariant();
                categories = categories
                    .Where(c => c.Name.ToLowerInvariant().Contains(searchLower) ||
                                (c.Products != null && c.Products.Any(p => p.Name.ToLowerInvariant().Contains(searchLower))))
                    .ToList();
            }

            var totalCount = categories.Count;

            // Apply pagination
            categories = categories
                .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            await _cache.SetAsync(cacheKey,categories,TimeSpan.FromMinutes(10));
            return (categories.Adapt<List<CategoryResponse>>(), totalCount);
        }

        public async Task<CategoryResponse?> GetCategoryById(Guid id)
        {
            var cacheKey = $"Category_{id}";
            var cached =  await _cache.GetAsync<CategoryResponse>(cacheKey);
            if (cached is not null) return cached;
            var category = await _categoryRepo.GetCategoryWithProductsById(id);
            var result = category?.Adapt<CategoryResponse>();
            if (result is not null)
                await _cache.SetAsync(cacheKey,result,TimeSpan.FromMinutes(10));
            return result;
        }

        public async Task CreateNewCategory(CategoryRequest request)
        {
            var category = request.Adapt<Category>();
            category.Slug = slugHelper.GenerateSlug(request.Name);
            await _unit.Categories.AddAsync(category);
            await _unit.SaveChangesAsync();
        }
        public async Task UpdateCategory(Guid id, CategoryRequest request)
        {
            var category = request.Adapt<Category>();
            category.Slug = slugHelper.GenerateSlug(request.Name);
            _unit.Categories.Update(id, category);
            await _unit.SaveChangesAsync();
        }

        public async Task DeleteCategory(Guid id)
        {
            _unit.Categories.Delete(id);
            await _unit.SaveChangesAsync();
        }

        public async Task<CategoryResponse?> GetCategoryBySlug(string slug)
        {
            var cacheKey = $"Category_Slug_{slug}";
            var cached = await _cache.GetAsync<CategoryResponse>(cacheKey);
            if (cached is not null) return cached;

            var category = await _categoryRepo.GetCategoryBySlug(slug);
            var result = category?.Adapt<CategoryResponse>();
            if (result is not null)
                await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10));
            return result;
        }

        public async Task<List<CategoryResponse>> GetSubCategoriesByCategoryId(Guid categoryId)
        {
            var cacheKey = $"SubCategories_Category_{categoryId}";
            var cached = await _cache.GetAsync<List<CategoryResponse>>(cacheKey);
            if (cached is not null) return cached;

            var categories = await _categoryRepo.GetAllCategoriesWithProducts();
            var category = categories.FirstOrDefault(c => c.Id == categoryId);
            var result = category?.SubCategories?.Adapt<List<CategoryResponse>>() ?? new List<CategoryResponse>();
            await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10));
            return result;
        }

        public async Task<List<CategoryResponse>> GetParentCategories()
        {
            var cacheKey = "ParentCategories";
            var cached = await _cache.GetAsync<List<CategoryResponse>>(cacheKey);
            if (cached is not null) return cached;

            var categories = await _categoryRepo.GetAllCategoriesWithProducts();
            var parents = categories.Where(c => c.ParentCategoryId == null).ToList();
            var result = parents.Adapt<List<CategoryResponse>>();
            await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10));
            return result;
        }

        public async Task<List<CategoryResponse>> GetAllSubCategories()
        {
            var cacheKey = "AllSubCategories";
            var cached = await _cache.GetAsync<List<CategoryResponse>>(cacheKey);
            if (cached is not null) return cached;

            var categories = await _categoryRepo.GetAllCategoriesWithProducts();
            var subs = categories.Where(c => c.ParentCategoryId != null).ToList();
            var result = subs.Adapt<List<CategoryResponse>>();
            await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10));
            return result;
        }

    }
}
