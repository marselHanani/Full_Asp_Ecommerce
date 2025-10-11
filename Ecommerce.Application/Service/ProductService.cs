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
        FileService fileService)
    {
        private readonly IUnitOfWork _unit = unit;
        private readonly SlugHelper _slugHelper = slugHelper;
        private readonly FileUrlHelper _helper = helper;
        private readonly IProductRepository _productRepo = productRepo;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly FileService _fileService = fileService;

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

        public async Task<IEnumerable<ProductResponse>> GetAllProducts()
        {
            var products = await _productRepo.GetAllProductsWithDetails();
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

            return responses;
        }

        public async Task<ProductResponse?> GetProductById(Guid id)
        {
            var product = await _productRepo.GetProductWithDetailsById(id);
            return await BuildProductResponse(product);
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
            var product = await _productRepo.GetProductWithCategorySlug(slug);
            return await BuildProductResponse(product);
        }

        public async Task<ProductResponse?> GetProductByBrandSlug(string slug)
        {
            var product = await _productRepo.GetProductWithBrandSlug(slug);
            return await BuildProductResponse(product);
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
