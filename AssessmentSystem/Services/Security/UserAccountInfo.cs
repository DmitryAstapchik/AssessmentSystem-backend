using System;
using System.Collections.Generic;

namespace AssessmentSystem.Services.Security
{
    /// <summary>
    /// Represents a user account.
    /// </summary>
    public class UserAccountInfo
    {
        /// <summary>
        /// Gets or sets a user account identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets a user profile identifier.
        /// </summary>
        public Guid ProfileId { get; set; }

        /// <summary>
        /// Gets or sets a user email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets a user roles names.
        /// </summary>
        public IEnumerable<string> Roles { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether user is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether a user email is confirmed.
        /// </summary>
        public bool IsEmailConfirmed { get; set; }
    }
}