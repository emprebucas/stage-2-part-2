using EcommerceApp.DTOs;
using FluentValidation;

namespace EcommerceApp.Validators
{
    /// <summary>
    /// 
    /// </summary>
    public class OrderValidator : AbstractValidator<OrderDto>
    {
        /// <summary>
        /// 
        /// </summary>
        public OrderValidator()
        {
            RuleFor(order => order.OrderId).NotEmpty();
            RuleFor(order => order.UserId).NotEmpty().WithMessage("Please add the destination country");
            RuleFor(order => order.Status).IsInEnum().WithMessage("Not enum");
        }
    }
}