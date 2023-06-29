using EcommerceApp.Commands;
using FluentValidation;

namespace EcommerceApp.Validators
{
    /// <summary>
    /// 
    /// </summary>
    public class AddUserCommandValidator : AbstractValidator<AddUserCommand>
    {
        /// <summary>
        /// 
        /// </summary>
        public AddUserCommandValidator()
        {
            RuleFor(newUser => newUser.User)
                .NotEmpty().WithMessage("'User' should not be emptaayyyy.");
            //RuleFor(newUser => newUser.UserId)
            //    .NotEmpty().WithMessage("'UserId' should not be emptaayyyy.");
            //RuleFor(newUser => newUser.Name)
            //    .NotEmpty().WithMessage("'Name' should not be emptyyyy.");
        }
    }
}
