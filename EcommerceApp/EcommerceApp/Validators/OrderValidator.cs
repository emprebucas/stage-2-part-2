using EcommerceApp.DTOs;
using FluentValidation;

namespace EcommerceApp.Validators
{
    /// <summary>
    /// OrderValidator is validator class for the `OrderDto` class using FluentValidation.
    /// It validates instances of the `OrderDto` class and ensure that the 
    /// `OrderId`, `UserId`, and `Status` properties meet the specified validation rules.
    /// </summary>
    public class OrderValidator : AbstractValidator<OrderDto>
    {
        /// <summary>
        /// The OrderValidator constructor has validation rules that are defined using the `RuleFor` method.
        /// </summary>
        public OrderValidator()
        {
            RuleFor(order => order.OrderId)
                .NotEmpty().WithMessage("'UserId' should not be empty.");
            RuleFor(order => order.UserId)
                .NotEmpty().WithMessage("'UserId' should not be empty.");
            RuleFor(order => order.Status)
                .NotNull().IsInEnum().WithMessage("Invalid status.");
        }
    }
}