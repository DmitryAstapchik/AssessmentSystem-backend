using FluentValidation;

namespace AssessmentSystem.Models.Validators
{
    /// <summary>
    /// Represents a validator for <see cref="PaginationParameters"/>.
    /// </summary>
    public class PaginationParametersValidator : AbstractValidator<PaginationParameters>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaginationParametersValidator"/> class.
        /// </summary>
        public PaginationParametersValidator()
        {
            RuleFor(x => x.Page).GreaterThan(0);
            RuleFor(x => x.Amount).GreaterThan(0);
        }
    }
}
