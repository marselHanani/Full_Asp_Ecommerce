using FluentValidation;
using Ecommerce.Application.Dtos.Request;

namespace Ecommerce.Application.Validators
{
    public class RemoveCartItemRequestValidator : AbstractValidator<RemoveCartItemRequest>
    {
        public RemoveCartItemRequestValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.");

            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Product ID is required.");
        }
    }
}
