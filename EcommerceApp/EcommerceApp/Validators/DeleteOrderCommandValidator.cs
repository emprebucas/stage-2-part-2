using EcommerceApp.Commands;
using FluentValidation;

namespace EcommerceApp.Validators
{
    /// <summary>
    /// 
    /// </summary>
    public class DeleteOrderCommandValidator : AbstractValidator<DeleteOrderCommand>

    {
        /// <summary>
        /// 
        /// </summary>
        public DeleteOrderCommandValidator()
        {
            RuleFor(deleteOrder => deleteOrder.OrderId).NotEmpty().WithMessage("Order");
        }

    }
}
