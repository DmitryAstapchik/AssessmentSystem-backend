using AssessmentSystem.Models.Security;
using FluentValidation;

namespace AssessmentSystem.Models.Validators
{
    /// <summary>
    /// Represents a validator for a request to reset a password .
    /// </summary>
    public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
    {
        public ResetPasswordRequestValidator()
        {
            RuleFor(c => c.Code)
                .NotEmpty().WithMessage("Code is required.");
            RuleFor(c => c.UserId)
                .NotEmpty().WithMessage("UserId is required.");
            RuleFor(с => с.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must contain minimum 8 symbols.");
            RuleFor(с => с.ConfirmPassword)
                .NotEmpty().WithMessage("Please confirm your password.")
                .Equal(с => с.Password).WithMessage("Passwords do not match");
        }
    }
}
