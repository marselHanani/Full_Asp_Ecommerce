using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Application.Dtos.Request;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Ecommerce.Application.Validators
{
    partial class ProductRequestValidator : AbstractValidator<ProductRequest>
    {
        private readonly string[] _allowedImageExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private readonly int _maxFileSizeBytes = 5 * 1024 * 1024; // 5MB

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

            RuleFor(p => p.Currency)
                .NotEmpty()
                .WithMessage("Currency is required.")
                .MaximumLength(3)
                .WithMessage("Currency code should not exceed 3 characters.");

            RuleFor(p => p.Stock)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Stock quantity cannot be negative.");

            RuleFor(p => p.CategoryId)
                .NotEqual(Guid.Empty)
                .WithMessage("Category must be selected.");

            RuleFor(p => p.BrandId)
                .NotEqual(Guid.Empty)
                .WithMessage("Brand must be selected.");

            When(p => p.MainImage != null, () =>
            {
                RuleFor(p => p.MainImage)
                    .Must(ValidateImageFile)
                    .WithMessage($"Main image must be valid image file (allowed extensions: {string.Join(", ", _allowedImageExtensions)}, max size: 5MB)");
            });

            When(p => p.SubImages != null && p.SubImages.Any(), () =>
            {
                RuleForEach(p => p.SubImages)
                    .Must(ValidateImageFile)
                    .WithMessage($"All sub-images must be valid image files (allowed extensions: {string.Join(", ", _allowedImageExtensions)}, max size: 5MB)");
            });
        }

        private bool ValidateImageFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            if (file.Length > _maxFileSizeBytes)
                return false;

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return _allowedImageExtensions.Contains(extension);
        }
    }
}
