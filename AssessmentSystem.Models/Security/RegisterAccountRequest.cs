using System;
using AssessmentSystem.Models.Validators;
using FluentValidation.Attributes;

namespace AssessmentSystem.Models.Security
{
    /// <summary>
    /// Contains information about new user account.
    /// </summary>
    [Validator(typeof(RegisterAccountRequestValidator))]
    public class RegisterAccountRequest
    {
        /// <summary>
        /// Gets or sets a first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets a last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets an email address.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets a password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets a confirm password.
        /// </summary>
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Gets or sets an invite code.
        /// </summary>
        public Guid? InviteCode { get; set; }
    }
}