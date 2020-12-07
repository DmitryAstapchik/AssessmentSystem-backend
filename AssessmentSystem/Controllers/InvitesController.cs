using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Http;
using AssessmentSystem.Models.UserManagement;
using AssessmentSystem.Services.Identity;
using AssessmentSystem.Services.Security;
using AssessmentSystem.Services.UserManagement;
using Swashbuckle.Swagger.Annotations;

namespace AssessmentSystem.Controllers
{
    /// <summary>
    /// Represents an invites api controller.
    /// </summary>
    [RoutePrefix("api/v1/invites")]
    [Authorize(Roles = GlobalInfo.Admin)]
    public class InvitesController : ApiController
    {
        private readonly IInvitesService _invitesService;
        private readonly IUserRoleService _userRoleService;
        private readonly ApplicationUserManager _applicationUserManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvitesController"/> class.
        /// </summary>
        /// <param name="invitesService">An invite service.</param>
        /// <param name="userRoleService">An user role service.</param>
        /// <param name="userContextService">An user context service.</param>
        /// <param name="applicationUserManager">An application user manager.</param>
        public InvitesController(IInvitesService invitesService, IUserRoleService userRoleService, ApplicationUserManager applicationUserManager)
        {
            _invitesService = invitesService ?? throw new ArgumentNullException(nameof(invitesService));
            _userRoleService = userRoleService ?? throw new ArgumentNullException(nameof(userRoleService));
            _applicationUserManager = applicationUserManager ?? throw new ArgumentNullException(nameof(applicationUserManager));
        }

        /// <summary>
        /// Gets all invites that were not used.
        /// </summary>
        /// <param name="emailPart">Part of an email string.</param>
        /// <returns>Result of getting invites.</returns>
        // GET /api/v1/invites?emailPart=
        [Route("")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error")]
        public IHttpActionResult GetUnusedInvites(string emailPart)
        {
            return Ok(_invitesService.GetUnusedInvites(emailPart));
        }

        /// <summary>
        /// Gets an active invite by token.
        /// </summary>
        /// <param name="token">An invite token.</param>
        /// <returns>An active invite if found.</returns>
        // GET /api/v1/invites/{token}
        [Route("{token:guid}")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "An active invite is not found.")]
        [AllowAnonymous]
        public IHttpActionResult GetActiveInvite(Guid token)
        {
            var invite = _invitesService.FindInviteByToken(token);
            if (invite == null || _invitesService.IsExpired(token))
            {
                return NotFound();
            }

            return Ok(invite);
        }

        /// <summary>
        /// Sends an invite to a coach.
        /// </summary>
        /// <param name="email">An email address of an invite recipient.</param>
        /// <returns>A <see cref="Task"/> with invite sending result.</returns>
        // POST /api/v1/invites/coach
        [HttpPost]
        [Route("coach")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The e-mail is not valid")]
        [SwaggerResponse(HttpStatusCode.Conflict, Description = "The user already has a role coach")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "An internal server error")]
        public Task<IHttpActionResult> SendInviteToCoach(string email)
        {
            return SendInviteAsync(new InviteInfo { Email = email, RoleName = GlobalInfo.Coach });
        }

        /// <summary>
        /// Sends an invite to a manager.
        /// </summary>
        /// <param name="email">An email address of an invite recipient.</param>
        /// <returns>A <see cref="Task"/> with invite sending result.</returns>
        // POST /api/v1/invites/manager
        [HttpPost]
        [Route("manager")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The e-mail is not valid")]
        [SwaggerResponse(HttpStatusCode.Conflict, Description = "The user already has a role manager")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "An internal server error")]
        public Task<IHttpActionResult> SendInviteToManager(string email)
        {
            return SendInviteAsync(new InviteInfo { Email = email, RoleName = GlobalInfo.Manager });
        }

        /// <summary>
        /// Verifies an invite and redirects to registration page.
        /// </summary>
        /// <param name="token">An invite token.</param>
        /// <returns>Verifying result</returns>
        [HttpGet]
        [Route("verify", Name = "RedirectInvite")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> VerifyInviteAsync([FromUri]Guid token)
        {
            try
            {
                var invite = _invitesService.FindInviteByToken(token);

                if (invite == null)
                {
                    return Redirect(new Uri(GlobalInfo.InviteNotFoundUrl));
                }

                if (_invitesService.IsExpired(token))
                {
                    return Redirect(new Uri(GlobalInfo.InviteIsExpiredUrl));
                }

                var userId = (await _applicationUserManager.FindByEmailAsync(invite.Email))?.Id;

                if (userId != null)
                {
                    var isInRole = await _applicationUserManager.IsInRoleAsync(userId, invite.RoleName);

                    if (isInRole)
                    {
                        return Redirect(new Uri(GlobalInfo.UserAlreadyInRoleUrl));
                    }
                    else
                    {
                        await _applicationUserManager.AddToRoleAsync(userId, invite.RoleName);
                        await _invitesService.UseInviteAsync(token, userId);
                        return Redirect(new Uri(GlobalInfo.UserAddedToRoleUrl));
                    }
                }

                return Redirect(new Uri($"{GlobalInfo.RegisterByInviteUrl}{token}"));
            }
            catch (Exception)
            {
                return Redirect(new Uri(GlobalInfo.InviteError));
            }
        }

        private async Task<IHttpActionResult> SendInviteAsync(InviteInfo inviteInfo)
        {
            try
            {
                new MailAddress(inviteInfo.Email);
            }
            catch (Exception)
            {
                return BadRequest("Not a valid e-mail.");
            }

            try
            {
                var userId = (await _applicationUserManager.FindByEmailAsync(inviteInfo.Email))?.Id;
                if (userId != null && await _applicationUserManager.IsInRoleAsync(userId, inviteInfo.RoleName))
                {
                    return Conflict();
                }

                var inviteId = await _invitesService.CreateInviteAsync(inviteInfo);
                var inviteLink = Url.Link("RedirectInvite", null);
                await _invitesService.SendInviteAsync(inviteId, inviteLink);

                return Ok();
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
    }
}
