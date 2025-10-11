using Ecommerce.Application.Dtos.Request;
using Ecommerce.Domain.Entity;
using Mapster;

namespace Ecommerce.Application.MappingConfig
{
    public class ProductMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<ProductRequest, Product>()
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.Description, src => src.Description)
                .Map(dest => dest.Stock, src => src.Stock)
                .Map(dest => dest.CategoryId, src => src.CategoryId)
                .Map(dest => dest.BrandId, src => src.BrandId)
                .Map(dest => dest.Price, src => new Money(src.Price,src.Currency));


        }
    
    }
}
