using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Application.Dtos.Request;
using FluentValidation;

namespace Ecommerce.Application.Validators
{
    public class OrderRequestValidator : AbstractValidator<OrderRequest>
    {
        public OrderRequestValidator()
        {
            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("At least one item must be added to the order");

            RuleFor(x => x.ShippingAddress)
                .NotEmpty().WithMessage("Shipping address is required");

            RuleFor(x => x.PaymentMethod)
                .IsInEnum().WithMessage("Invalid payment method selected");

            RuleForEach(x => x.Items)
                .SetValidator(new OrderItemRequestValidator());
        }
    }
}
