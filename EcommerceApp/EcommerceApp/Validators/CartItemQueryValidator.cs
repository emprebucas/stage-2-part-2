using EcommerceApp.Queries;
using FluentValidation;

namespace EcommerceApp.Validators
{
    /// <summary>
    /// 
    /// </summary>
    public class CartItemQueryValidator : AbstractValidator<CartItemQuery>

    {
        /// <summary>
        /// 
        /// </summary>
        public CartItemQueryValidator()
        {
            //RuleFor(findOrder => findOrder.OrderId).NotEmpty().WithMessage("UserId");
        }

    }
}
