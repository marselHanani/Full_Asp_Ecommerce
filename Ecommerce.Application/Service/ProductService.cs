using Ecommerce.Application.Dtos.Request;
using Ecommerce.Application.Dtos.Response;
using Ecommerce.Application.Helper;
using Ecommerce.Domain.Entity;
using Ecommerce.Domain.Repository.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Http;
using Slugify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Application.Service
{
    public class ProductService(
        IUnitOfWork unit,
        SlugHelper slugHelper,
        FileUrlHelper helper,
        IProductRepository productRepo,
        IHttpContextAccessor httpContextAccessor,
        FileService fileService,
        CacheService cache)
    {
        private readonly IUnitOfWork _unit = unit;
        private readonly SlugHelper _slugHelper = slugHelper;
        private readonly FileUrlHelper _helper = helper;
        private readonly IProductRepository _productRepo = productRepo;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly FileService _fileService = fileService;
        private readonly CacheService _cache = cache;

        public async Task CreateNewProduct(ProductRequest request)
        {
            var product = request.Adapt<Product>();
            product.Slug = _slugHelper.GenerateSlug(request.Name);

            await _unit.Products.AddAsync(product);

            var productImages = new List<ProductImage>();

            if (request.MainImage != null)
            {
                var fileName = await _fileService.SaveFileAsync(request.MainImage, "Products");
                productImages.Add(new ProductImage
                {
                    ProductId = product.Id,
                    FileName = fileName,
                    IsMain = true
                });
            }

            if (request.SubImages != null && request.SubImages.Any())
            {
                foreach (var subImage in request.SubImages)
                {
                    var fileName = await _fileService.SaveFileAsync(subImage, "Products");
                    productImages.Add(new ProductImage
                    {
                        ProductId = product.Id,
                        FileName = fileName,
                        IsMain = false
                    });
                }
            }

            if (productImages.Any())
                await _unit.ProductImages.AddRangeAsync(productImages);

            await _unit.SaveChangesAsync();
        }

        public async Task<(IEnumerable<ProductResponse> Products, int TotalCount)> GetAllProducts(
            string? search = null,
            int page = 1,
            int pageSize = 10,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            Guid? categoryId = null,
            Guid? brandId = null)
        {
            // Build cache key based on all filter parameters
            var cacheKey = $"products:{search}:{page}:{pageSize}:{minPrice}:{maxPrice}:{categoryId}:{brandId}";
            var cachedResult = await _cache.GetAsync<List<ProductResponse>>(cacheKey);

            if (cachedResult != null)
            {
                return (cachedResult, cachedResult.Count);
            }

            var products = await _productRepo.GetAllProductsWithDetails();

            // Search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.Trim().ToLowerInvariant();
                products = products
                    .Where(p => p.Name.ToLowerInvariant().Contains(searchLower) ||
                                (p.Description != null && p.Description.ToLowerInvariant().Contains(searchLower)) ||
                                (p.Category != null && p.Category.Name.ToLowerInvariant().Contains(searchLower)) ||
                                (p.Brand != null && p.Brand.Name.ToLowerInvariant().Contains(searchLower)))
                    .ToList();
            }

            // Price filter
            if (minPrice.HasValue)
            {
                products = products.Where(p => p.Price.Amount >= minPrice.Value).ToList();
            }
            if (maxPrice.HasValue)
            {
                products = products.Where(p => p.Price.Amount <= maxPrice.Value).ToList();
            }

            // Category filter
            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value).ToList();
            }

            // Brand filter
            if (brandId.HasValue)
            {
                products = products.Where(p => p.BrandId == brandId.Value).ToList();
            }

            var totalCount = products.Count;

            // Pagination
            products = products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var responses = products.Adapt<List<ProductResponse>>();
            var request = _httpContextAccessor.HttpContext!.Request;

            foreach (var product in responses)
            {
                if (product.Images != null && product.Images.Any())
                {
                    var mainImage = product.Images.FirstOrDefault(img => img.IsMain);
                    product.MainImageUrl = mainImage != null
                        ? _helper.GetImageUrl(mainImage.FileName, "Products", request)
                        : string.Empty;

                    product.Images = product.Images
                        .Where(img => !img.IsMain)
                        .Select(img => new ProductImageResponse
                        {
                            FileName = img.FileName,
                            IsMain = img.IsMain,
                            Url = _helper.GetImageUrl(img.FileName, "Products", request)
                        })
                        .ToList();
                }
                else
                {
                    product.MainImageUrl = string.Empty;
                    product.Images = new List<ProductImageResponse>();
                }
            }

            await _cache.SetAsync(cacheKey, (responses, totalCount), TimeSpan.FromMinutes(10));

            return (responses, totalCount);
        }

        public async Task<ProductResponse?> GetProductById(Guid id)
        {
            var cacheKey = $"product:{id}";
            var cachedProduct = await _cache.GetAsync<ProductResponse>(cacheKey);
            if (cachedProduct != null)
                return cachedProduct;

            var product = await _productRepo.GetProductWithDetailsById(id);
            var response = await BuildProductResponse(product);

            if (response != null)
                await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(10));

            return response;
        }

        public async Task UpdateProduct(Guid id, ProductRequest request)
        {
            // Get existing product with images
            var existingProduct = await _unit.Products.GetByIdAsync(id);
            if (existingProduct == null)
                throw new Exception("Product not found");

            // Handle Main Image
            if (request.MainImage != null)
            {
                // Find and delete previous main image
                var prevMainImage = existingProduct.Images?.FirstOrDefault(img => img.IsMain);
                if (prevMainImage != null)
                {
                    _fileService.DeleteFile(prevMainImage.FileName, "Products");
                    _unit.ProductImages.Delete(prevMainImage.Id);
                }

                // Save new main image
                var newMainFileName = await _fileService.SaveFileAsync(request.MainImage, "Products");
                var newMainImage = new ProductImage
                {
                    ProductId = existingProduct.Id,
                    FileName = newMainFileName,
                    IsMain = true
                };
                await _unit.ProductImages.AddAsync(newMainImage);
            }

            // Handle Sub Images
            if (request.SubImages != null && request.SubImages.Any())
            {
                // Delete previous sub images
                var prevSubImages = existingProduct.Images?.Where(img => !img.IsMain).ToList();
                if (prevSubImages != null)
                {
                    foreach (var img in prevSubImages)
                    {
                        _fileService.DeleteFile(img.FileName, "Products");
                        _unit.ProductImages.Delete(img.Id);
                    }
                }

                // Save new sub images
                foreach (var subImage in request.SubImages)
                {
                    var subFileName = await _fileService.SaveFileAsync(subImage, "Products");
                    var newSubImage = new ProductImage
                    {
                        ProductId = existingProduct.Id,
                        FileName = subFileName,
                        IsMain = false
                    };
                    await _unit.ProductImages.AddAsync(newSubImage);
                }
            }

            // Update product fields (excluding images)
            existingProduct.Name = request.Name;
            existingProduct.Slug = _slugHelper.GenerateSlug(request.Name);
            existingProduct.Description = request.Description;
            existingProduct.Price = new Money(request.Price, request.Currency);
            existingProduct.Stock = request.Stock;
            existingProduct.CategoryId = request.CategoryId;
            existingProduct.BrandId = request.BrandId;
            existingProduct.UpdatedAt = DateTime.UtcNow;

            _unit.Products.Update(id, existingProduct);
            await _unit.SaveChangesAsync();
        }

        public async Task DeleteProduct(Guid id)
        {
            _unit.Products.Delete(id);
            await _unit.SaveChangesAsync();
        }

        public async Task<ProductResponse?> GetProductByCategorySlug(string slug)
        {
            var cacheKey = $"product:category:{slug}";
            var cachedProduct = await _cache.GetAsync<ProductResponse>(cacheKey);
            if (cachedProduct != null)
                return cachedProduct;

            var product = await _productRepo.GetProductWithCategorySlug(slug);
            var response = await BuildProductResponse(product);

            if (response != null)
                await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(10));

            return response;
        }

        public async Task<ProductResponse?> GetProductByBrandSlug(string slug)
        {
            var cacheKey = $"product:brand:{slug}";
            var cachedProduct = await _cache.GetAsync<ProductResponse>(cacheKey);
            if (cachedProduct != null)
                return cachedProduct;

            var product = await _productRepo.GetProductWithBrandSlug(slug);
            var response = await BuildProductResponse(product);

            if (response != null)
                await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(10));

            return response;
        }

        private async Task<ProductResponse?> BuildProductResponse(Product? product)
        {
            if (product == null) return null;

            var response = product.Adapt<ProductResponse>();
            var request = _httpContextAccessor.HttpContext!.Request;

            if (product.Images != null && product.Images.Any())
            {
                var mainImage = product.Images.FirstOrDefault(i => i.IsMain);
                response.MainImageUrl = mainImage != null
                    ? _helper.GetImageUrl(mainImage.FileName, "Products", request)
                    : string.Empty;

                response.Images = product.Images
                    .Where(img => !img.IsMain)
                    .Select(img => new ProductImageResponse
                    {
                        FileName = img.FileName,
                        IsMain = img.IsMain,
                        Url = _helper.GetImageUrl(img.FileName, "Products", request)
                    })
                    .ToList();
            }

            return response;
        }
    }
}
