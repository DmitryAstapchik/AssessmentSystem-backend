using System;

namespace AssessmentSystem.Services.Security
{
    /// <summary>
    /// Represents a user profile.
    /// </summary>
    public class UserProfileInfo
    {
        /// <summary>
        /// Gets or sets a user profile identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets a user firstname.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets a user lastname.
        /// </summary>
        public string LastName { get; set; }
    }
}
