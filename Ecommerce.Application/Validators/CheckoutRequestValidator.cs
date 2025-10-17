using FluentValidation;
using Ecommerce.Application.Dtos.Request;

namespace Ecommerce.Application.Validators
{
    public class CheckoutRequestValidator : AbstractValidator<CheckoutRequest>
    {
        public CheckoutRequestValidator()
        {
            RuleFor(x => x.ShippingAddress)
                .NotEmpty().WithMessage("Shipping address is required")
                .MaximumLength(200).WithMessage("Shipping address must not exceed 200 characters")
                .MinimumLength(5).WithMessage("Shipping address must be at least 5 characters");

            When(x => x.UserId != null, () =>
            {
                RuleFor(x => x.UserId)
                    .NotEmpty().WithMessage("User ID cannot be empty when provided");
            });
        }
    }
}
