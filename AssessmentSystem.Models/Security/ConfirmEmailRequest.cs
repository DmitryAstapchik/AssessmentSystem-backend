namespace AssessmentSystem.Models.Security
{
    /// <summary>
    /// Represents a model that can be used by a user for sending a request to confirm his or her email.
    /// </summary>
    public class ConfirmEmailRequest
    {
        /// <summary>
        /// Gets or sets a user id.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets a confrimation code.
        /// </summary>
        public string Code { get; set; }
    }
}
