using EcommerceApp.DTOs;
using FluentValidation;

namespace EcommerceApp.Validators
{
    /// <summary>
    /// 
    /// </summary>
    public class CartItemValidator : AbstractValidator<CartItemDto>
    {
        /// <summary>
        /// 
        /// </summary>
        public CartItemValidator()
        {
            RuleFor(cartItem => cartItem.CartItemId)
                .NotEmpty().WithMessage("'CartItemId' should not be empty.");
            RuleFor(cartItem => cartItem.OrderId)
                .NotEmpty().WithMessage("'OrderId' should not be empty.");
            RuleFor(cartItem => cartItem.UserId)
                .NotEmpty().WithMessage("'UserId' should not be empty.");
            RuleFor(cartItem => cartItem.Item)
                .NotEmpty().WithMessage("'Item' should not be empty.");
            RuleFor(cartItem => cartItem.Price)
                .NotEmpty().GreaterThan(0).WithMessage("'Price' should not be empty.");
        }
    }
}
