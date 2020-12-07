namespace AssessmentSystem.Models.UserManagement
{
    /// <summary>
    /// Represents information about invite.
    /// </summary>
    public class InviteInfo
    {
        public InviteInfo()
        {
        }

        public InviteInfo(string email, string roleName)
        {
            Email = email;
            RoleName = roleName;
        }

        /// <summary>
        /// Gets or sets an email address.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets a role name.
        /// </summary>
        public string RoleName { get; set; }
    }
}
