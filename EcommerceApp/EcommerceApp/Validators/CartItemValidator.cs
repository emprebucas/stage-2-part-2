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
            RuleFor(cartItem => cartItem.CartItemId).NotEmpty().WithMessage("CartItemId");
            RuleFor(cartItem => cartItem.OrderId).NotEmpty().WithMessage("OrderId");
            RuleFor(cartItem => cartItem.UserId).NotEmpty().WithMessage("UserId");
            RuleFor(cartItem => cartItem.Item).NotEmpty().WithMessage("Item aaahhhhhh");
            RuleFor(cartItem => cartItem.Price).NotEmpty().WithMessage("Price");
        }
    }
}
