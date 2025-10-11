using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Ecommerce.API.Extensions
{
    public static class FluentValidationExtensions
    {
        public static IServiceCollection AddFluentValidation(this IServiceCollection services)
        {
            // Use AddValidatorsFromAssembly instead of AddValidatorsFromAssemblyContaining
            services.AddValidatorsFromAssembly(Assembly.Load("Ecommerce.Application"));
            return services;
        }
    }
}
