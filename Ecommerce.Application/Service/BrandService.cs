using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Application.Dtos.Request;
using Ecommerce.Application.Dtos.Response;
using Ecommerce.Application.Helper;
using Ecommerce.Domain.Entity;
using Ecommerce.Domain.Repository.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Http;
using Slugify;

namespace Ecommerce.Application.Service
{
    public class BrandService(IUnitOfWork unit , SlugHelper slugHelper , IBrandRepository brandRepo
        , FileService fileService,FileUrlHelper helper, IHttpContextAccessor http, CacheService cache)
    {
        private readonly IUnitOfWork _unit = unit;
        private readonly SlugHelper _slugHelper = slugHelper;
        private readonly IBrandRepository _brandRepo = brandRepo;
        private readonly FileService _fileService = fileService;
        private readonly FileUrlHelper _helper = helper;
        private readonly IHttpContextAccessor _http = http;
        private readonly CacheService _cache = cache;

        public async Task CreateNewBrand(BrandRequest request)
        {

            var brand = request.Adapt<Brand>();
            brand.Slug = _slugHelper.GenerateSlug(request.Name);
            if (request.Image is not null)
                brand.ImageName = await _fileService.SaveFileAsync(request.Image, "Brands");
            await _unit.Brands.AddAsync(brand);
            await _unit.SaveChangesAsync();
        }

        public async Task UpdateBrand(Guid id, BrandRequest request)
        {
            var existingBrand = await _unit.Brands.GetByIdAsync(id);
            if (existingBrand is null)
                throw new Exception("Brand not found");

            existingBrand.Name = request.Name;
            existingBrand.Slug = _slugHelper.GenerateSlug(request.Name);

            if (request.Image is not null)
            {
                if (!string.IsNullOrEmpty(existingBrand.ImageName))
                    _fileService.DeleteFile(existingBrand.ImageName, "Brands");
                existingBrand.ImageName = await _fileService.SaveFileAsync(request.Image, "Brands");
            }

            _unit.Brands.Update(id, existingBrand);
            await _unit.SaveChangesAsync();
        }

        public async Task<IEnumerable<BrandResponse>> GetAllBrands(string? search = null)
        {
            var brands = await _brandRepo.GetBrandsWithProducts();
            const string cacheKey = "Brands_all";
            var cached = await _cache.GetAsync<IEnumerable<BrandResponse>>(cacheKey);
            if (cached != null) return cached;
            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(search))
            {
                brands = brands
                    .Where(b => b.Name.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            var result = brands.Adapt<List<BrandResponse>>();
            var request = _http.HttpContext!.Request;

            for (int i = 0; i < brands.Count; i++)
            {
                result[i].ImageUrl = _helper.GetImageUrl(brands[i].ImageName, "Brands", request);
            }

            await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10));
            return result;
        }
        public async Task<BrandResponse?> GetBrandById(Guid id)
        {
            var cacheKey = $"brand_{id}";
            var cached = await _cache.GetAsync<BrandResponse>(cacheKey);
            if (cached != null) return cached;
            var brand = await _brandRepo.GetBrandByIdWithProducts(id);
            var result = brand?.Adapt<BrandResponse>();
            result.ImageUrl = _helper.GetImageUrl(brand.ImageName, "Brands", _http.HttpContext.Request);
            await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10));
            return result;
        }
        public async Task DeleteBrand(Guid id)
        {
            _unit.Brands.Delete(id);
            await _unit.SaveChangesAsync();
        }
    }
}
