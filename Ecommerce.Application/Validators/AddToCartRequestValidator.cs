using Ecommerce.Application.Dtos.Request;
using FluentValidation;

namespace Ecommerce.Application.Validators
{
    public class AddToCartRequestValidator : AbstractValidator<AddToCartRequest>
    {
        public AddToCartRequestValidator()
        {
            RuleFor(request => request.ProductId)
                .NotEmpty().WithMessage("Product ID is required.");

            RuleFor(request => request.Quantity)
                .NotEmpty().WithMessage("Quantity is required.")
                .GreaterThan(0).WithMessage("Quantity must be greater than 0.")
                .LessThanOrEqualTo(100).WithMessage("Quantity cannot exceed 100 items per order.");
        }
    }
}
