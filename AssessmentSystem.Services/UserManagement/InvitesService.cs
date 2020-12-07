using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using AssessmentSystem.Data.Access.Context.Interfaces;
using AssessmentSystem.Data.Access.UserManagement;
using AssessmentSystem.Models.UserManagement;
using AssessmentSystem.Services.Exceptions;

namespace AssessmentSystem.Services.UserManagement
{
    /// <summary>
    /// Represents an invites service.
    /// </summary>
    public class InvitesService : IInvitesService
    {
        private readonly TimeSpan _inviteLifespan = TimeSpan.FromDays(2);
        private readonly IInvitesContext _context;
        private readonly IEmailSendingService _emailSendingService;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvitesService"/> class.
        /// </summary>
        /// <param name="context">An invites context.</param>
        /// <param name="emailSendingService">An email sending service.</param>
        public InvitesService(IInvitesContext context, IEmailSendingService emailSendingService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _emailSendingService = emailSendingService ?? throw new ArgumentNullException(nameof(emailSendingService));
        }

        /// <summary>
        /// Creates an invite.
        /// </summary>
        /// <param name="inviteInfo">An invite info.</param>
        /// <returns>A created invite id.</returns>
        /// <exception cref="ArgumentNullException">Email address or role is null.</exception>
        public async Task<int> CreateInviteAsync(InviteInfo inviteInfo)
        {
            if (inviteInfo == null)
            {
                throw new ArgumentNullException(nameof(inviteInfo));
            }

            new MailAddress(inviteInfo.Email);

            if (inviteInfo.RoleName == null)
            {
                throw new ArgumentNullException(nameof(inviteInfo.RoleName));
            }

            var invite = _context.Invites.SingleOrDefault(i =>
                i.Email.Equals(inviteInfo.Email, StringComparison.OrdinalIgnoreCase) &&
                i.RoleName.Equals(inviteInfo.RoleName, StringComparison.OrdinalIgnoreCase));

            if (invite != null)
            {
                invite.Token = Guid.NewGuid();
                invite.ExpiredDate = DateTime.Now + _inviteLifespan;
            }
            else
            {
                invite = _context.Invites.Add(new Invite
                {
                    Token = Guid.NewGuid(),
                    Email = inviteInfo.Email,
                    RoleName = inviteInfo.RoleName,
                    ExpiredDate = DateTime.Now + _inviteLifespan
                });
            }

            await _context.SaveChangesAsync();

            return invite.Id;
        }

        /// <summary>
        /// Searches for an invite by token.
        /// </summary>
        /// <param name="token">An invite token.</param>
        /// <returns>An invite info.</returns>
        public InviteInfo FindInviteByToken(Guid token)
        {
            var invite = _context.Invites.SingleOrDefault(i => i.Token == token);
            return invite != null ? new InviteInfo(invite.Email, invite.RoleName) : null;
        }

        /// <summary>
        /// Marks that an invite is used by some user.
        /// </summary>
        /// <param name="inviteToken">An invite token.</param>
        /// <param name="userId">A user id.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task UseInviteAsync(Guid inviteToken, string userId)
        {
            var invite = _context.Invites.SingleOrDefault(i => i.Token == inviteToken);
            if (invite == null)
            {
                throw new InviteNotFoundException();
            }

            invite.UserId = userId;
            invite.ExpiredDate = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Gets invites that were not used.
        /// </summary>
        /// <returns><see cref="IEnumerable{InviteInfo}"/></returns>
        /// <param name="emailPart">Part of an email string.</param>
        public IEnumerable<InviteInfo> GetUnusedInvites(string emailPart)
        {
            if (string.IsNullOrWhiteSpace(emailPart))
            {
                return new InviteInfo[0];
            }

            return _context.Invites
                .Where(i => i.UserId == null)
                .Where(i => i.Email.ToLower().Contains(emailPart.ToLower()))
                .OrderBy(i => i.Email)
                .Select(i => new InviteInfo { Email = i.Email, RoleName = i.RoleName });
        }

        /// <summary>
        /// Checks an invite on expiration.
        /// </summary>
        /// <param name="token">An invite token.</param>
        /// <returns>Returns a value indicating whether the invite is expired.</returns>
        /// <exception cref="InviteNotFoundException">An invite is not found.</exception>
        public bool IsExpired(Guid token)
        {
            var invite = _context.Invites.SingleOrDefault(x => x.Token == token);
            if (invite == null)
            {
                throw new InviteNotFoundException();
            }

            return invite.ExpiredDate < DateTime.Now;
        }

        /// <summary>
        /// Sends an invite to the recipient's email address.
        /// </summary>
        /// <param name="inviteId">An invite token.</param>
        /// <param name="inviteLink">An invite link.</param>
        /// <returns>A task of asynchronous invite sending.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Invite id less than zero.</exception>
        /// <exception cref="ArgumentNullException">Action link is null.</exception>
        /// <exception cref="InviteNotFoundException">Invite not found.</exception>
        /// <exception cref="InvalidOperationException">The invite expired date and time less than current date and time.</exception>
        public Task SendInviteAsync(int inviteId, string inviteLink)
        {
            if (string.IsNullOrWhiteSpace(inviteLink))
            {
                throw new ArgumentNullException(nameof(inviteLink));
            }

            var invite = _context.Invites.SingleOrDefault(i => i.Id == inviteId);
            if (invite == null)
            {
                throw new InviteNotFoundException($"Invite with id {inviteId} is not found.");
            }

            if (invite.ExpiredDate < DateTime.Now)
            {
                throw new InvalidOperationException($"Invite with id {inviteId} is expired.");
            }

            var message = new MailMessage("from@email.com", invite.Email)
            {
                Body = $"You are invited as a {invite.RoleName}: <a href='{inviteLink}?token={invite.Token}'>accept invite</a>",
                Subject = "Invite",
                IsBodyHtml = true
            };

            return _emailSendingService.SendAsync(message);
        }
    }
}
