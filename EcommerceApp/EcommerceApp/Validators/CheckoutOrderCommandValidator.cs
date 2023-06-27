using EcommerceApp.Commands;
using FluentValidation;

namespace EcommerceApp.Validators
{
    /// <summary>
    /// 
    /// </summary>
    public class CheckoutOrderCommandValidator : AbstractValidator<CheckoutOrderCommand>

    {
        /// <summary>
        /// 
        /// </summary>
        public CheckoutOrderCommandValidator()
        {
            RuleFor(checkoutOrder => checkoutOrder.Order).NotEmpty().WithMessage("Order");
        }

    }
}
