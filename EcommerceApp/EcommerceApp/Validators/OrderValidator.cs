using EcommerceApp.DTOs;
using FluentValidation;
using System;

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
            RuleFor(order => order.OrderId)
                .NotEmpty().WithMessage("'UserId' should not be empty.");
            RuleFor(order => order.UserId)
                .NotEmpty().WithMessage("'UserId' should not be empty.");
            RuleFor(order => order.Status)
                .NotNull().IsInEnum().WithMessage("Invalid status.");
        }
    }
}