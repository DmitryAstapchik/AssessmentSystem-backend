using System;

namespace AssessmentSystem.Data.Access.UserManagement
{
    /// <summary>
    /// Represents an invite.
    /// </summary>
    public class Invite
    {
        /// <summary>
        /// Gets or sets an invite id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a token.
        /// </summary>
        public Guid Token { get; set; }

        /// <summary>
        /// Gets or sets an email address.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets a role name.
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// Gets or sets an invite expiration date.
        /// </summary>
        public DateTime ExpiredDate { get; set; }

        /// <summary>
        /// Gets or sets an id of the user registered by this invite.
        /// </summary>
        public string UserId { get; set; }
    }
}
