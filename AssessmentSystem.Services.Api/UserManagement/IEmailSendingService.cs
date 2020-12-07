using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace AssessmentSystem.Services.UserManagement
{
    /// <summary>
    /// Represents an email sending service.
    /// </summary>
    public interface IEmailSendingService : IDisposable
    {
        /// <summary>
        /// Sends a mail message asynchronously.
        /// </summary>
        /// <param name="mailMessage">An e-mail message to send.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous email sending.</returns>
        Task SendAsync(MailMessage mailMessage);
    }
}
