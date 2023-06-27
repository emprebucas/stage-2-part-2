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
            RuleFor(user => user.UserId != Guid.Empty)
                //.Must(BeValidGuid).WithMessage("'UserId' is not a valid GUID.")
                .NotEmpty().WithMessage("'UserId' should not be empty.");
            RuleFor(user => user.Name)
                .NotEmpty().WithMessage("'Name' should not be empty.");
        }

        private bool BeValidGuid(Guid userId)
        {
            return Guid.TryParse(userId.ToString(), out _);
        }
    }
}
