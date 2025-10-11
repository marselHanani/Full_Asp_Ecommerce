using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Application.Dtos.Request;
using FluentValidation;

namespace Ecommerce.Application.Validators
{
    partial class ProductRequestValidator : AbstractValidator<ProductRequest>
    {
        public ProductRequestValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty()
                .WithMessage("Product name is required.");

            RuleFor(p => p.Price)
                .GreaterThan(0)
                .WithMessage("Product price must be greater than zero.");

            RuleFor(p => p.Description)
                .MaximumLength(500)
                .WithMessage("Product description must not exceed 500 characters.");
        }
    }
}
