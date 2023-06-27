using EcommerceApp.Commands;
using FluentValidation;

namespace EcommerceApp.Validators
{
    /// <summary>
    /// 
    /// </summary>
    public class DeleteCartItemCommandValidator : AbstractValidator<DeleteCartItemCommand>

    {
        /// <summary>
        /// 
        /// </summary>
        public DeleteCartItemCommandValidator()
        {
            RuleFor(deleteCartItem => deleteCartItem.CartItemId).NotEmpty().WithMessage("CartItem");
        }

    }
}
