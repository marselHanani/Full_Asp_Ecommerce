using Ecommerce.Application.Dtos.Response;
using Ecommerce.Application.Helper;
using Ecommerce.Application.Service;
using Ecommerce.Domain.Entity;
using Ecommerce.Domain.Repository.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Http;
using Moq;
using Slugify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Tests.Services
{
    public class ProductServiceTests
    {
        [Fact]
        public async Task GetAllProducts_ReturnProducts()
        {
            TypeAdapterConfig<Money, decimal>.NewConfig().MapWith(m => m.Amount);
            // Arrange
            var mockUnit = new Mock<IUnitOfWork>();
            var mockProductRepo = new Mock<IProductRepository>();
            var mockGenericProductRepo = new Mock<IGenericRepository<Product>>();
            var mockSlugHelper = new Mock<SlugHelper>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var mockFileService = new Mock<FileService>();

            // Use a test double for FileUrlHelper since GetImageUrl is not virtual
            var fileUrlHelper = new TestFileUrlHelper();

            // Setup HttpContext and Request for FileUrlHelper
            var mockHttpContext = new Mock<HttpContext>();
            var mockRequest = new Mock<HttpRequest>();
            mockHttpContext.Setup(x => x.Request).Returns(mockRequest.Object);
            mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(mockHttpContext.Object);

            // Setup Product with required navigation properties
            var product = new Product
            {
                Name = "Test",
                Slug = "test",
                Description = "desc",
                Price = new Money { Amount = 10, Currency = "USD" },
                Category = new Category { Name = "Cat", Slug = "cat" },
                Brand = new Brand { Name = "Brand", Slug = "brand" }
            };

            mockProductRepo.Setup(r => r.GetAllProductsWithDetails())
                .ReturnsAsync(new List<Product> { product });

            mockUnit.Setup(u => u.Products).Returns(mockGenericProductRepo.Object);

            var service = new ProductService(
                mockUnit.Object,
                mockSlugHelper.Object,
                fileUrlHelper,
                mockProductRepo.Object,
                mockHttpContextAccessor.Object,
                mockFileService.Object
            );

            // Act
            var (result, totalCount) = await service.GetAllProducts();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        // Test double for FileUrlHelper
        private class TestFileUrlHelper : FileUrlHelper
        {
            public string GetImageUrl(string fileName, string folderName, HttpRequest request)
            {
                return "http://localhost/images/test.jpg";
            }
        }
    }
}
