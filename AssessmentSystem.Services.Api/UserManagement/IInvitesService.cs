using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AssessmentSystem.Models.UserManagement;

namespace AssessmentSystem.Services.UserManagement
{
    /// <summary>
    /// Represents a service for creating personal invites.
    /// </summary>
    public interface IInvitesService
    {
        /// <summary>
        /// Creates an invite.
        /// </summary>
        /// <param name="inviteInfo">An invite info.</param>
        /// <returns>A created invite id.</returns>
        Task<int> CreateInviteAsync(InviteInfo inviteInfo);

        /// <summary>
        /// Sends an invite.
        /// </summary>
        /// <param name="inviteId">An invite id.</param>
        /// <param name="inviteLink">An invite link.</param>
        /// <returns>A task of asynchronous invite sending.</returns>
        Task SendInviteAsync(int inviteId, string inviteLink);

        /// <summary>
        /// Gets invites that were not used.
        /// </summary>
        /// <returns><see cref="IEnumerable{InviteInfo}"/></returns>
        /// <param name="emailPart">Part of an email string</param>
        IEnumerable<InviteInfo> GetUnusedInvites(string emailPart);

        /// <summary>
        /// Checks an invite on expiration.
        /// </summary>
        /// <param name="token">An invite token.</param>
        /// <returns>Returns a value indicating whether the invite is expired.</returns>
        bool IsExpired(Guid token);

        /// <summary>
        /// Finds an invite by token.
        /// </summary>
        /// <param name="token">An invite token.</param>
        /// <returns>An invite info.</returns>
        InviteInfo FindInviteByToken(Guid token);

        /// <summary>
        /// Marks that an invite is used by some user.
        /// </summary>
        /// <param name="inviteToken">An invite token.</param>
        /// <param name="userId">A user id.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task UseInviteAsync(Guid inviteToken, string userId);
    }
}
