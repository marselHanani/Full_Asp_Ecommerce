using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Application.Dtos.Request;
using Ecommerce.Domain.Entity;
using Ecommerce.Domain.Repository.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Http;

namespace Ecommerce.Application.Service
{
    public class ProductImageService(IUnitOfWork _unit, FileService service)
    {
        private readonly IUnitOfWork _unit = _unit;
        private readonly FileService _service = service;

        public async Task<ProductImage> AddImageAsync(ProductImageRequest request)
        {
            var fileName = await _service.SaveFileAsync(request.Image, "Products");
            var image = new ProductImage
            {
                ProductId = request.ProductId,
                FileName = fileName,
                IsMain = request.IsMain
            };
            await _unit.ProductImages.AddAsync(image);
            await _unit.SaveChangesAsync();
            return image;
        }

        public async Task DeleteImageAsync(Guid imageId)
        {
            var image = await _unit.ProductImages.GetByIdAsync(imageId);
            if (image == null)
            {
                throw new Exception("Image not found");
            }
            _service.DeleteFile(image.FileName, "Products");
            _unit.ProductImages.Delete(imageId);
            await _unit.SaveChangesAsync();
        }

        // add many images for product by product id
        public async Task<List<ProductImage>> AddImagesAsync(Guid productId, List<IFormFile> images)
        {
            var imageList = new List<ProductImage>();
            foreach (var img in images)
            {
                var fileName = await _service.SaveFileAsync(img, "Products");
                var image = new ProductImage
                {
                    ProductId = productId,
                    FileName = fileName,
                    IsMain = false
                };
                imageList.Add(image);
            }
            await _unit.ProductImages.AddRangeAsync(imageList);
            await _unit.SaveChangesAsync();
            return imageList;
        }
    }
}
