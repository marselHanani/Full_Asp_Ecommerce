using Ecommerce.Application.Dtos.Response;
using Ecommerce.Domain.Entity;
using Mapster;

namespace Ecommerce.Application.MappingConfig;

public class ReviewResponseMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Reply mapping
        config.NewConfig<Review, ReplyResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Content, src => src.Content)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt)
            .Map(dest => dest.Replies, src => src.Reviews);

        config.NewConfig<Review, ReviewResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Title, src => src.Title)
            .Map(dest => dest.Content, src => src.Content)
            .Map(dest => dest.ProductName, src => src.Product.Name)
            .Map(dest => dest.Rating, src => src.Rating)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt)
            .Map(dest => dest.Comments, src => src.Reviews);
    }
}