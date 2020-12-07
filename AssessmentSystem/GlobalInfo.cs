using System.Configuration;

namespace AssessmentSystem
{
    /// <summary>
    /// Contains global information.
    /// </summary>
    public static class GlobalInfo
    {
        /// <summary>
        /// Admin role name.
        /// </summary>
        public const string Admin = "admin";

        /// <summary>
        /// Candidate role name.
        /// </summary>
        public const string Candidate = "candidate";

        /// <summary>
        /// Coach role name.
        /// </summary>
        public const string Coach = "coach";

        /// <summary>
        /// Manager role name.
        /// </summary>
        public const string Manager = "manager";

        /// <summary>
        /// Stores frontend host name.
        /// </summary>
        public static readonly string FrontendHost = ConfigurationManager.AppSettings.Get("FrontendHost");

        /// <summary>
        /// Stores url to page for confirm email.
        /// </summary>
        public static readonly string ConfirmEmailUrl = FrontendHost + "restorePassword/emailSent";

        /// <summary>
        /// Stores url to page for restore password.
        /// </summary>
        public static readonly string RestorePasswordUrl = FrontendHost + "restorePassword/enterNew";

        /// <summary>
        /// Stores url to the page with invite not found notification.
        /// </summary>
        public static readonly string InviteNotFoundUrl = FrontendHost + "invites/notifications/notFound";

        /// <summary>
        /// Stores url to the page with invite expired notification.
        /// </summary>
        public static readonly string InviteIsExpiredUrl = FrontendHost + "invites/notifications/isExpired";

        /// <summary>
        /// Stores url to the page with user already in role notification.
        /// </summary>
        public static readonly string UserAlreadyInRoleUrl = FrontendHost + "invites/notifications/alreadyInRole";

        /// <summary>
        /// Stores url to the page with user added to role notification.
        /// </summary>
        public static readonly string UserAddedToRoleUrl = FrontendHost + "invites/notifications/addedToRole";

        /// <summary>
        /// Stores url to the page for registration by invite.
        /// </summary>
        public static readonly string RegisterByInviteUrl = FrontendHost + "signUp?code=";

        /// <summary>
        /// Stores url to the page with invite error notification.
        /// </summary>
        public static readonly string InviteError = FrontendHost + "invites/notifications/error";
    }
}
