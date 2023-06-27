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
            RuleFor(newUser => newUser.User).NotEmpty().WithMessage("User");
        }

    }
}
