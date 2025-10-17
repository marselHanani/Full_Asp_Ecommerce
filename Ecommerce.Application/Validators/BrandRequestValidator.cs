using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Application.Dtos.Request;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Ecommerce.Application.Validators
{
    public class BrandRequestValidator : AbstractValidator<BrandRequest>
    {
        public BrandRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Brand name is required")
                .MaximumLength(100).WithMessage("Brand name cannot exceed 100 characters");

            RuleFor(x => x.Image)
                .NotNull().WithMessage("Brand image is required")
                .Must(BeValidImage).WithMessage("Please select a valid image file (jpg, jpeg, png, or gif).")
                .Must(BeValidSize).WithMessage("Image size should not exceed 5MB");
        }

        private bool BeValidImage(IFormFile file)
        {
            if (file == null)
                return false;

            // Check if the file is an image
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            return allowedExtensions.Contains(extension) && 
                   (file.ContentType.StartsWith("image/") || file.ContentType == "application/octet-stream");
        }

        private bool BeValidSize(IFormFile file)
        {
            if (file == null)
                return false;

            // Max size: 5MB
            const int maxSizeInBytes = 5 * 1024 * 1024;
            return file.Length <= maxSizeInBytes;
        }
    }
}
