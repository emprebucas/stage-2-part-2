using EcommerceApp.Commands;
using FluentValidation;

namespace EcommerceApp.Validators
{
    /// <summary>
    /// 
    /// </summary>
    public class UpdateCartItemCommandValidator : AbstractValidator<UpdateCartItemCommand>

    {
        /// <summary>
        /// 
        /// </summary>
        public UpdateCartItemCommandValidator()
        {
            RuleFor(updateCartItem => updateCartItem.CartItem).NotEmpty().WithMessage("CartItem");
        }

    }
}
