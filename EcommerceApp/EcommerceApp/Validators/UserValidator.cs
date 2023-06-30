using EcommerceApp.DTOs;
using FluentValidation;

namespace EcommerceApp.Validators
{
    /// <summary>
    /// UserValidator is validator class for the `UserDto` class using FluentValidation.
    /// It validates instances of the `UserDto` class and ensure that the 
    /// `UserId` and `Name` properties meet the specified validation rules.
    /// </summary>
    public class UserValidator : AbstractValidator<UserDto>
    {
        /// <summary>
        /// The UserValidator constructor has validation rules that are defined using the `RuleFor` method.
        /// </summary>
        public UserValidator()
        {
            RuleFor(user => user.UserId)
                .NotEmpty().WithMessage("'UserId' should not be empty.");
            RuleFor(user => user.Name)
                .NotEmpty().WithMessage("'Name' should not be empty.");
        }
    }

}
