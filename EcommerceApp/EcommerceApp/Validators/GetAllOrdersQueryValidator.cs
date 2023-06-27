using EcommerceApp.Queries;
using FluentValidation;

namespace EcommerceApp.Validators
{
    /// <summary>
    /// 
    /// </summary>
    public class GetAllOrdersQueryValidator : AbstractValidator<GetAllOrdersQuery>

    {
        /// <summary>
        /// 
        /// </summary>
        public GetAllOrdersQueryValidator()
        {
            //RuleFor(findOrder => findOrder.OrderId).NotEmpty().WithMessage("UserId");
        }

    }
}
