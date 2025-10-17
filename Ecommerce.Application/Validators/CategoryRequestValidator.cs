using System;
using FluentValidation;
using Ecommerce.Application.Dtos.Request;

namespace Ecommerce.Application.Validators
{
    public class CategoryRequestValidator : AbstractValidator<CategoryRequest>
    {
        public CategoryRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category name is required")
                .MaximumLength(100).WithMessage("Category name must not exceed 100 characters");

            // ParentCategoryId is optional, but if provided, it must be a valid GUID
            When(x => x.ParentCategoryId.HasValue, () =>
            {
                RuleFor(x => x.ParentCategoryId.Value)
                    .NotEqual(Guid.Empty).WithMessage("Parent category ID cannot be an empty GUID");
            });
        }
    }
}
