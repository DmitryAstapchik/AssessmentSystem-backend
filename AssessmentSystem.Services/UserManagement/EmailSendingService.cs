using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace AssessmentSystem.Services.UserManagement
{
    /// <summary>
    /// Represents an email sending service.
    /// </summary>
    internal class EmailSendingService : IEmailSendingService
    {
        private bool _disposed = false;
        private SmtpClient _smtpClient;

        /// <summary>
        /// Disposes an instance of <see cref="EmailSendingService"/>
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Sends a mail message asynchronously.
        /// </summary>
        /// <param name="mailMessage">A mail</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous email sending.</returns>
        public Task SendAsync(MailMessage mailMessage)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(EmailSendingService));
            }

            _smtpClient = new SmtpClient();
            return _smtpClient.SendMailAsync(mailMessage);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            try
            {
                if (disposing)
                {
                    _smtpClient?.Dispose();
                    GC.SuppressFinalize(this);
                }
            }
            finally
            {
                _disposed = true;
            }
        }
    }
}