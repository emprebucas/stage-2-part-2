using EcommerceApp.DTOs;
using FluentValidation;

namespace EcommerceApp.Validators
{
    /// <summary>
    /// CartItemValidator is validator class for the `CartItemDto` class using FluentValidation.
    /// It validates instances of the `CartItemDto` class and ensure that the 
    /// `CartItemId`, `OrderId`, `UserId`, `Item`, and `Price` properties meet the specified validation rules.
    /// </summary>
    public class CartItemValidator : AbstractValidator<CartItemDto>
    {
        /// <summary>
        /// The CartItemValidator constructor has validation rules that are defined using the `RuleFor` method.
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
                .NotEmpty().GreaterThan(0).WithMessage("'Price' should not be empty and should be greater than 0.");
        }
    }
}
