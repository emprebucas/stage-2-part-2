using EcommerceApp.Commands;
using FluentValidation;

namespace EcommerceApp.Validators
{
    /// <summary>
    /// 
    /// </summary>
    public class AddCartItemCommandValidator : AbstractValidator<AddCartItemCommand>

    {
        /// <summary>
        /// 
        /// </summary>
        public AddCartItemCommandValidator()
        {
            RuleFor(newCartItem => newCartItem.CartItem).NotEmpty().WithMessage("CartItem");
        }
        
    }
}
