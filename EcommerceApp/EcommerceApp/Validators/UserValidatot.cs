using EcommerceApp.DTOs;
using FluentValidation;

namespace EcommerceApp.Validators
{
    /// <summary>
    /// 
    /// </summary>
    public class UserValidator : AbstractValidator<UserDto>
    {
        /// <summary>
        /// 
        /// </summary>
        public UserValidator()
        {
            RuleFor(user => user.UserId).NotEmpty().WithMessage("UserId");
            RuleFor(user => user.Name).NotEmpty().WithMessage("Name aaahhhhhh");
        }
    }
}
