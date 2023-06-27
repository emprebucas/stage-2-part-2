using EcommerceApp.Queries;
using FluentValidation;

namespace EcommerceApp.Validators
{
    /// <summary>
    /// 
    /// </summary>
    public class GetUserByIdQueryValidator : AbstractValidator<GetUserByIdQuery>

    {
        /// <summary>
        /// 
        /// </summary>
        public GetUserByIdQueryValidator()
        {
            RuleFor(findUser => findUser.UserId).NotEmpty().WithMessage("UserId");
        }

    }
}
