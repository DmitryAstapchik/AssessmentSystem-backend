using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AssessmentSystem.Identity;
using AssessmentSystem.Models;
using AssessmentSystem.Models.Security;
using AssessmentSystem.Models.UserManagement;
using AssessmentSystem.Services.Identity;
using AssessmentSystem.Services.Security;
using AssessmentSystem.Services.UserManagement;
using Microsoft.AspNet.Identity;
using Swashbuckle.Swagger.Annotations;

namespace AssessmentSystem.Controllers
{
    /// <summary>
    /// Controller for accounts.
    /// </summary>
    [RoutePrefix("api/v1/accounts")]
    public class AccountsController : ApiController
    {
        private readonly ApplicationUserManager _userManager;
        private readonly IEmailSendingService _emailSendingService;
        private readonly IInvitesService _invitesService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountsController"/> class.
        /// </summary>
        /// <param name="userManager">An instance of <see cref="ApplicationUserManager"/>.</param>
        /// <param name="emailSendingService">Email sending service.</param>
        /// <param name="invitesService">Invite service.</param>
        public AccountsController(ApplicationUserManager userManager, IEmailSendingService emailSendingService, IInvitesService invitesService)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _emailSendingService = emailSendingService ?? throw new ArgumentNullException(nameof(emailSendingService));
            _invitesService = invitesService ?? throw new ArgumentNullException(nameof(invitesService));
        }

        /// <summary>
        /// Checks whether the user is authorized.
        /// </summary>
        /// <returns>Status code 200 if the user is logged in.</returns>
        // GET /api/v1/check/login
        [HttpGet]
        [Authorize]
        [Route("check/login")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "User is unauthorized")]
        [SwaggerResponse(HttpStatusCode.OK, Description = "User is authorized")]
        public IHttpActionResult CheckLogin() => Ok();

        /// <summary>
        /// Creates a new user account.
        /// </summary>
        /// <param name="model">model with information about new user account</param>
        /// <returns>Result of creation.</returns>
        // POST /api/v1/accounts body: { firstName, lastName, email, password, confirmPassword } -> { email, id, roles }
        [HttpPost]
        [AllowAnonymous]
        [Route("", Name = "Register")]
        [SwaggerResponse(HttpStatusCode.Created, Description = "A new user account is created")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Register data are not correct")]
        public async Task<IHttpActionResult> Register([FromBody]RegisterAccountRequest model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _userManager.FindByNameAsync(model.Email) != null)
            {
                return BadRequest("User with such email already exists");
            }

            var createResult = await CreateUser(model);
            IdentityResult result = createResult.Item1;
            Guid userDomainId = createResult.Item2;

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            var actionLink = Url.Link("GetUserAccountById", new { id = userDomainId });
            return Created(actionLink, new { Id = userDomainId });
        }

        /// <summary>
        /// Gets user accounts that are registered in system.
        /// </summary>
        /// <param name="parameters">Pagination parameters.</param>
        /// <param name="roleName">Users role name.</param>
        /// <param name="emailPart">Part of an email of a user to search users by email.</param>
        /// <param name="sortCriteria">Criteria of user sorting.</param>
        /// <param name="isDescending">Determines whether sorting is descending.</param>
        /// <returns>
        /// User accounts that are registered in system.
        /// </returns>
        /// <remarks>By default, a page index is 1, an amount is 25.</remarks>
        /// <exception cref="HttpResponseException">Page index is less or equals to zero or amount is less or equals to zero.</exception>
        // GET /api/v1/accounts
        // GET /api/v1/accounts?page=1&amount=25
        // GET /api/v1/accounts?page=1&amount=25&roleName='candidate'
        // GET /api/v1/accounts?page=1&amount=25&roleName='candidate'&emailPart='smith@example.com'
        // GET /api/v1/accounts?page=1&amount=25&roleName='candidate'&sortCriteria='email'&isDescending='false'
        [HttpGet]
        [Route("")]
        [ResponseType(typeof(UserAccountInfo[]))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Parameter values are not correct")]
        public IHttpActionResult GetUserAccounts(
            [FromUri(Name = "")] PaginationParameters parameters,
            [FromUri(Name = "roleName")] string roleName = null,
            [FromUri(Name = "emailPart")] string emailPart = null,
            [FromUri(Name = "sortCriteria")] string sortCriteria = nameof(ApplicationUser.Email),
            [FromUri(Name = "isDescending")] bool isDescending = false)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            parameters = parameters ?? new PaginationParameters();
            try
            {
                var accounts = _userManager.GetAccounts(
                    (parameters.Page - 1) * parameters.Amount,
                    parameters.Amount,
                    roleName,
                    emailPart,
                    sortCriteria,
                    isDescending);
                return Ok(accounts.ToArray());
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        /// <summary>
        /// Gets a count of user accounts that are registered in system.
        /// </summary>
        /// <param name="roleName">Name of users role.</param>
        /// <param name="emailPart">Part of an email of a user to search users by email.</param>
        /// <returns>
        /// Count of user accounts that are registered in system.
        /// </returns>
        // GET /api/v1/accounts/count
        // GET /api/v1/accounts/count?roleName='candidate'&emailPart='smith@example.com'
        [HttpGet]
        [Route("count")]
        [ResponseType(typeof(int))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error")]
        public IHttpActionResult GetUserAccountsCount(
            [FromUri(Name = "roleName")] string roleName = null,
            [FromUri(Name = "emailPart")] string emailPart = null)
        {
            try
            {
                var accountsCount = _userManager.GetAccountsCount(roleName, emailPart);
                return Ok(accountsCount);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        /// <summary>
        /// Send confirm email message.
        /// </summary>
        /// <param name="model">An instance of <see cref="ForgotPasswordRequest"/>.</param>
        /// <returns>Result of sending confirmation email message</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("confirm/email/send")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The email is not valid")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "An internal server error")]
        [SwaggerResponse(HttpStatusCode.OK, Description = "Email message for confirmation email sent successfully")]
        public async Task<IHttpActionResult> SendConfirmEmail([FromBody]ForgotPasswordRequest model)
        {
            try
            {
                new MailAddress(model.Email);
            }
            catch (Exception)
            {
                return BadRequest("Email is not valid.");
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound();
            }

            try
            {
                await SendConfirmEmailAsync(user);
            }
            catch (Exception)
            {
                return InternalServerError();
            }

            return Ok();
        }

        /// <summary>
        /// Confirms email.
        /// </summary>
        /// <param name="confirm">Contains user id and confirmation code in base64</param>
        /// <returns>Result of confirmation.</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("confirm/email")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Sent data is not correct")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error")]
        [SwaggerResponse(HttpStatusCode.OK, Description = "Email confirmed successfully")]
        public async Task<IHttpActionResult> ConfirmEmail([FromBody]ConfirmEmailRequest confirm)
        {
            if (confirm == null)
            {
                return BadRequest();
            }

            try
            {
                confirm.UserId = FromBase64(confirm.UserId);
                confirm.Code = FromBase64(confirm.Code);
            }
            catch (Exception)
            {
                return BadRequest();
            }

            if (await _userManager.FindByIdAsync(confirm.UserId) == null)
            {
                return BadRequest();
            }

            try
            {
                var result = await _userManager.ConfirmEmailAsync(confirm.UserId, confirm.Code);

                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }
            }
            catch (Exception)
            {
                return InternalServerError();
            }

            return Ok();
        }

        /// <summary>
        /// Confirms email by admin.
        /// </summary>
        /// <param name="id">Id of a target user.</param>
        /// <returns>Result of confirmation.</returns>
        [HttpPatch]
        [Authorize(Roles = GlobalInfo.Admin)]
        [Route("confirm/email/by-admin/{id}")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Sent data is not correct")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error")]
        [SwaggerResponse(HttpStatusCode.OK, Description = "Email confirmed successfully")]
        public async Task<IHttpActionResult> ConfirmEmailByAdmin(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest($"{nameof(id)} is an empty guid");
            }

            try
            {
                await _userManager.ConfirmEmailByAdminAsync(id);
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return InternalServerError();
            }

            return Ok();
        }

        /// <summary>
        /// Recover password.
        /// </summary>
        /// <param name="model">An instance of <see cref="ForgotPasswordRequest"/>.</param>
        /// <returns>Result of sending recovery email message</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("password/forgot")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The e-mail is not valid.")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "The user with given e-mail is not found in the system.")]
        [SwaggerResponse(HttpStatusCode.OK, Description = "E-mail message for recovery is sent successfully.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "An internal server error.")]
        public async Task<IHttpActionResult> ForgotPassword([FromBody]ForgotPasswordRequest model)
        {
            try
            {
                new MailAddress(model.Email);
            }
            catch (Exception)
            {
                return BadRequest("E-mail is not valid.");
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound();
            }

            try
            {
                await SendForgotPasswordAsync(user);
            }
            catch (Exception)
            {
                return InternalServerError();
            }

            return Ok();
        }

        /// <summary>
        /// Reset password
        /// </summary>
        /// <param name="model">Contains user id and confirmation code in base64, password, confrim password.</param>
        /// <returns>Result of reset.</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("password/reset")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Sent data is not correct")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error")]
        [SwaggerResponse(HttpStatusCode.OK, Description = "Email confirmed successfully")]
        public async Task<IHttpActionResult> ResetPassword([FromBody]ResetPasswordRequest model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                model.UserId = FromBase64(model.UserId);
                model.Code = FromBase64(model.Code);
            }
            catch (Exception)
            {
                return BadRequest("UserId or code is invalid.");
            }

            if (await _userManager.FindByIdAsync(model.UserId) == null)
            {
                return BadRequest("UserId or code is invalid.");
            }

            try
            {
                var result = await _userManager.ResetPasswordAsync(model.UserId, model.Code, model.Password);

                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }
            }
            catch (Exception)
            {
                return InternalServerError();
            }

            return Ok();
        }

        /// <summary>
        /// Gets a user account with specified identifier.
        /// </summary>
        /// <param name="id">An identifier of target user.</param>
        /// <returns>
        /// User account.
        /// </returns>
        // GET /api/v1/accounts/{id}
        [HttpGet]
        [Route("{id}", Name = "GetUserAccountById")]
        [ResponseType(typeof(UserAccountInfo))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Parameter values are not correct")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "User account can not be found")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error")]
        public async Task<IHttpActionResult> GetUserAccountById(string id)
        {
            if (!Guid.TryParse(id, out var guid) || guid == Guid.Empty)
            {
                return BadRequest($"{nameof(id)} is not a valid guid");
            }

            try
            {
                var account = await _userManager.GetAccountAsync(id);
                return Ok(account);
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        /// <summary>
        /// Changes value of IsActive field of user.
        /// </summary>
        /// <param name="id">An identifier of target user.</param>
        /// <returns>A changing operation result.</returns>
        // PATCH /api/v1/accounts/{id}/changeIsActive
        [HttpPatch]
        [Route("{id:guid}/changeIsActive", Name = "ChangeIsActive")]
        [ResponseType(typeof(void))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Parameter values are not correct")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "User account not found")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error")]
        public async Task<IHttpActionResult> ChangeIsActive(string id)
        {
            if (!Guid.TryParse(id, out var guid) || guid == Guid.Empty)
            {
                return BadRequest($"{nameof(id)} is not a valid guid");
            }

            try
            {
                await _userManager.ChangeIsActiveAsync(id);
                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        /// <summary>
        /// Get a user id by email.
        /// </summary>
        /// <param name="email">An email of target user.</param>
        /// <returns>User id.</returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("id")]
        [ResponseType(typeof(string))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Parameter values are not correct")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "User account can not be found")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error")]
        public async Task<IHttpActionResult> GetUserIdByEmail(string email)
        {
            try
            {
                new MailAddress(email);
            }
            catch (Exception)
            {
                return BadRequest("Email is not valid");
            }

            try
            {
                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    return NotFound();
                }

                return Ok(user.Id);
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        /// <summary>
        /// Gets a user roles names.
        /// </summary>
        /// <param name="id">An identifier of target user.</param>
        /// <returns>
        /// User roles names.
        /// </returns>
        // GET /api/v1/accounts/{id}/roles
        [HttpGet]
        [Route("{id}/roles")]
        [ResponseType(typeof(IEnumerable<string>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Parameter values are not correct")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "User account can not be found")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error")]
        public async Task<IHttpActionResult> GetUserRoles(string id)
        {
            if (!Guid.TryParse(id, out var guid) || guid == Guid.Empty)
            {
                return BadRequest($"{nameof(id)} is not a valid guid");
            }

            try
            {
                var account = await _userManager.GetAccountAsync(id);
                return Ok(account.Roles);
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        /// <summary>
        /// Generate http action result from identity result.
        /// </summary>
        /// <param name="result">identity result</param>
        /// <returns>Http action result which contains information from identity result.</returns>
        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                }

                if (ModelState.IsValid)
                {
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private async Task SendConfirmEmailAsync(ApplicationUser user)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user.Id);
            var message = new MailMessage("from@email.com", user.Email)
            {
                Body = $"Please confirm your account by clicking this link: <a href='{GlobalInfo.ConfirmEmailUrl}?userId={ToBase64(user.Id)}&code={ToBase64(code)}'>link</a>",
                Subject = "Confirm your account",
                IsBodyHtml = true
            };
            await _emailSendingService.SendAsync(message);
        }

        private async Task SendForgotPasswordAsync(ApplicationUser user)
        {
            var code = await _userManager.GeneratePasswordResetTokenAsync(user.Id);
            var message = new MailMessage("from@email.com", user.Email)
            {
                Body = $"Please reset your password by clicking here: <a href='{GlobalInfo.RestorePasswordUrl}?userId={ToBase64(user.Id)}&code={ToBase64(code)}'>link</a>",
                Subject = "Reset Password",
                IsBodyHtml = true
            };
            await _emailSendingService.SendAsync(message);
        }

        private string FromBase64(string input)
        {
            byte[] buffer = Convert.FromBase64String(input);
            return Encoding.ASCII.GetString(buffer);
        }

        private string ToBase64(string input)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(input);
            return Convert.ToBase64String(buffer);
        }

        private async Task<Tuple<IdentityResult, Guid>> CreateUser(RegisterAccountRequest model)
        {
            InviteInfo invite = null;

            if (model.InviteCode != null)
            {
                invite = _invitesService.FindInviteByToken(model.InviteCode.Value);
                if (invite == null || !invite.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase))
                {
                    return new Tuple<IdentityResult, Guid>(IdentityResult.Failed("Bad invite"), Guid.Empty);
                }
            }

            var user = new ApplicationUser
            {
                DomainId = Guid.NewGuid(),
                UserName = model.Email,
                Email = model.Email,
                IsActive = true,
                Profile = new ApplicationUserProfile { FirstName = model.FirstName, LastName = model.LastName }
            };

            IdentityResult result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return new Tuple<IdentityResult, Guid>(result, Guid.Empty);
            }

            string role = GlobalInfo.Candidate;
            if (invite != null)
            {
                role = invite.RoleName;
                user.EmailConfirmed = true;
                await _invitesService.UseInviteAsync(model.InviteCode.Value, user.Id);
            }
            else
            {
                await SendConfirmEmailAsync(user);
            }

            await _userManager.AddToRoleAsync(user.Id, role);

            return new Tuple<IdentityResult, Guid>(IdentityResult.Success, user.DomainId);
        }
    }
}
