using EcommerceApp.Queries;
using FluentValidation;

namespace EcommerceApp.Validators
{
    /// <summary>
    /// 
    /// </summary>
    public class GetOrderByIdQueryValidator : AbstractValidator<GetOrderByIdQuery>

    {
        /// <summary>
        /// 
        /// </summary>
        public GetOrderByIdQueryValidator()
        {
            RuleFor(findOrder => findOrder.OrderId).NotEmpty().WithMessage("UserId");
        }

    }
}
