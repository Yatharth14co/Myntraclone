using FluentValidation;
using ECommerceApi.DTOs;

namespace ECommerceApi.Validators;

/// <summary>
/// Validator for cart item creation
/// </summary>
public class CreateCartItemRequestValidator : AbstractValidator<CreateCartItemRequest>
{
    public CreateCartItemRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("Product ID must be greater than 0");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0")
            .LessThanOrEqualTo(1000)
            .WithMessage("Quantity cannot exceed 1000");
    }
}

/// <summary>
/// Validator for coupon application
/// </summary>
public class ApplyCouponRequestValidator : AbstractValidator<ApplyCouponRequest>
{
    public ApplyCouponRequestValidator()
    {
        RuleFor(x => x.CouponCode)
            .NotEmpty()
            .WithMessage("Coupon code is required")
            .Length(1, 50)
            .WithMessage("Coupon code must be between 1 and 50 characters")
            .Matches("^[A-Z0-9]+$")
            .WithMessage("Coupon code must contain only uppercase letters and numbers");
    }
}
