using System;
using FluentValidation;
using Ecommerce.Application.Dtos.Request;

namespace Ecommerce.Application.Validators
{
    public class PaymentRequestValidator : AbstractValidator<PaymentRequest>
    {
        public PaymentRequestValidator()
        {
            RuleFor(x => x.OrderId)
                .NotEmpty()
                .WithMessage("Order ID is required");

            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Amount must be greater than zero");

            RuleFor(x => x.Currency)
                .NotEmpty()
                .WithMessage("Currency is required")
                .Length(3)
                .WithMessage("Currency code must be exactly 3 characters")
                .Matches("^[A-Z]{3}$")
                .WithMessage("Currency code must consist of 3 uppercase letters");

            RuleFor(x => x.PaymentType)
                .IsInEnum()
                .WithMessage("Invalid payment type");
        }
    }
}
