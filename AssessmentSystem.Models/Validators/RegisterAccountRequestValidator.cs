using AssessmentSystem.Models.Security;
using FluentValidation;

namespace AssessmentSystem.Models.Validators
{
    /// <summary>
    /// Validator for user's register data.
    /// </summary>
    public class RegisterAccountRequestValidator : AbstractValidator<RegisterAccountRequest>
    {
        public RegisterAccountRequestValidator()
        {
            RuleFor(c => c.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .Length(2, 20).WithMessage("Minimum length is 2.");
            RuleFor(c => c.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .Length(2, 20).WithMessage("Minimum length is 2.");
            RuleFor(c => c.Email)
                .EmailAddress().WithMessage("Invalid email format.");
            RuleFor(с => с.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must contain minimum 8 symbols.");
            RuleFor(с => с.ConfirmPassword)
                .NotEmpty().WithMessage("Please confirm your password.")
                .Equal(с => с.Password).WithMessage("Passwords do not match");
        }
    }
}