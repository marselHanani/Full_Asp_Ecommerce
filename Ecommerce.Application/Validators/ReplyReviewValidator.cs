using FluentValidation;
using Ecommerce.Application.Dtos.Request;

namespace Ecommerce.Application.Validators
{
    internal class ReplyReviewValidator : AbstractValidator<ReplyReviewRequest>
    {
        public ReplyReviewValidator()
        {
            RuleFor(x => x.Comment)
                .NotEmpty().WithMessage("Comment is required")
                .MaximumLength(1000).WithMessage("Comment cannot exceed 1000 characters");

            RuleFor(x => x.ParentId)
                .NotEqual(Guid.Empty).WithMessage("Parent review ID is required");
        }
    }
}
