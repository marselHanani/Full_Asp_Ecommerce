using Ecommerce.Application.Dtos.Response;
using Ecommerce.Domain.Entity;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.MappingConfig
{
    public class CartMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // 🛒 من Cart إلى CartResponse
            config.NewConfig<Cart, CartResponse>()
                .Map(dest => dest.CartId, src => src.Id) // لأن BaseEntity عندك فيه Id
                .Map(dest => dest.Items, src => src.Items)
                .Map(dest => dest.TotalPrice, src => src.TotalPrice);

            // 🧾 من CartItem إلى CartItemResponse
            config.NewConfig<CartItem, CartItemResponse>()
                .Map(dest => dest.ProductId, src => src.ProductId)
                .Map(dest => dest.ProductName, src => src.Product.Name)
                .Map(dest => dest.UnitPrice, src => src.UnitPrice)
                .Map(dest => dest.Quantity, src => src.Quantity)
                .Map(dest => dest.Total, src => src.Total);
        }
    }
}
