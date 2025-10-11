using Ecommerce.Application.Dtos.Response;
using Ecommerce.Domain.Entity;
using Mapster;

namespace Ecommerce.Application.MappingConfig;

public class ProductResponseMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ProductImage, ProductImageResponse>()
            .Map(dest => dest.FileName, src => src.FileName)
            .Map(dest => dest.IsMain, src => src.IsMain);

        config.NewConfig<Product, ProductResponse>()
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Slug, src => src.Slug)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.Stock, src => src.Stock)
            .Map(dest => dest.CategoryName, src => src.Category.Name)
            .Map(dest => dest.BrandName, src => src.Brand.Name)
            .Map(dest => dest.Price, src => src.Price.Amount)
            .Map(dest => dest.Currency, src => src.Price.Currency)
            .Map(dest => dest.Images, src => src.Images);

        TypeAdapterConfig<OrderItem, OrderItemResponse>.NewConfig()
            .Map(dest => dest.ProductId, src => src.ProductId)
            .Map(dest => dest.ProductName, src => src.Product.Name)
            .Map(dest => dest.Quantity, src => src.Quantity)
            .Map(dest => dest.UnitPrice, src => src.UnitPrice);

        // ثانياً: تحويل Order -> OrderResponse
        TypeAdapterConfig<Order, OrderResponse>.NewConfig()
            .Map(dest => dest.Status, src => src.Status.ToString())
            .Map(dest => dest.UserId, src => src.UserId.ToString())
            .Map(dest => dest.Items, src => src.Items.Adapt<List<OrderItemResponse>>());
    }
}