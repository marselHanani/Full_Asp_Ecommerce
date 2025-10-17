using Ecommerce.Application.Dtos.Request;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;

namespace Ecommerce.Application.Validators
{
    public class ProductImageRequestValidator : AbstractValidator<ProductImageRequest>
    {
        public ProductImageRequestValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage("Product ID is required");

            RuleFor(x => x.Image)
                .NotNull()
                .WithMessage("Image file is required");

            RuleFor(x => x.Image)
                .Must(BeValidImageFile)
                .When(x => x.Image != null)
                .WithMessage("File must be a valid image (jpg, jpeg, png, gif)");
            
            RuleFor(x => x.Image)
                .Must(HaveValidFileSize)
                .When(x => x.Image != null)
                .WithMessage("Image file size must be less than 5MB");
        }

        private bool BeValidImageFile(IFormFile file)
        {
            if (file == null) return false;

            // Check if the file is an image by its content type
            var allowedTypes = new[] 
            { 
                "image/jpeg", 
                "image/jpg", 
                "image/png", 
                "image/gif" 
            };

            return allowedTypes.Contains(file.ContentType.ToLower());
        }

        private bool HaveValidFileSize(IFormFile file)
        {
            if (file == null) return false;
            
            // Limit file size to 5MB
            const int maxFileSizeInBytes = 5 * 1024 * 1024;
            return file.Length <= maxFileSizeInBytes;
        }
    }
}
