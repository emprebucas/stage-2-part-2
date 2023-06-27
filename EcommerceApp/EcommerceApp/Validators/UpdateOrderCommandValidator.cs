using EcommerceApp.Commands;
using FluentValidation;

namespace EcommerceApp.Validators
{
    /// <summary>
    /// 
    /// </summary>
    public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>

    {
        /// <summary>
        /// 
        /// </summary>
        public UpdateOrderCommandValidator()
        {
            RuleFor(checkoutOrder => checkoutOrder.Order).NotEmpty().WithMessage("Order");
        }

    }
}
