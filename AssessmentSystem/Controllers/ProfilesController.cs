using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AssessmentSystem.Filters;
using AssessmentSystem.Models;
using AssessmentSystem.Services.Identity;
using AssessmentSystem.Services.Security;
using Swashbuckle.Swagger.Annotations;

namespace AssessmentSystem.Controllers
{
    /// <summary>
    /// Represents a controller for user profiles management.
    /// </summary>
    [RoutePrefix("api/v1/profiles")]
    public class ProfilesController : ApiController
    {
        private readonly ApplicationUserManager _userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfilesController"/> class.
        /// </summary>
        /// <param name="userManager">An instance of <see cref="ApplicationUserManager"/>.</param>
        public ProfilesController(ApplicationUserManager userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        /// <summary>
        /// Gets user profiles that are registered in system.
        /// </summary>
        /// <param name="parameters">Pagination parameters.</param>
        /// <param name="roleName">Users role name.</param>
        /// <returns>
        /// User profiles that are registered in system.
        /// </returns>
        /// <remarks>By default, a page index is 1, an amount is 25.</remarks>
        /// <exception cref="HttpResponseException">Page index is less or equals to zero or amount is less or equals to zero.</exception>
        // GET /api/v1/profiles
        // GET /api/v1/profiles?page=1&amount=25
        // GET /api/v1/profiles?page=1&amount=25&roleName='candidate'
        [HttpGet]
        [Route("")]
        [ModelStateValidationFilter]
        [ResponseType(typeof(UserProfileInfo[]))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Parameter values are not correct")]
        public IHttpActionResult GetUserProfiles(
            [FromUri(Name = "")] PaginationParameters parameters,
            [FromUri(Name = "roleName")]string roleName = null)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            parameters = parameters ?? new PaginationParameters();
            try
            {
                var profiles = _userManager.GetProfiles((parameters.Page - 1) * parameters.Amount, parameters.Amount, roleName);
                return Ok(profiles.ToArray());
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        /// <summary>
        /// Gets a user profile with specified identifier.
        /// </summary>
        /// <param name="id">An identifier of target user.</param>
        /// <returns>
        /// User profile.
        /// </returns>
        // GET /api/v1/profiles/{id}
        [HttpGet]
        [Route("{id}")]
        [ResponseType(typeof(UserProfileInfo))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Parameter values are not correct")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "User profile can not be found")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error")]
        public async Task<IHttpActionResult> GetUserProfileById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest($"{nameof(id)} is an empty guid");
            }

            try
            {
                var profile = await _userManager.GetProfileAsync(id);
                return Ok(profile);
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }
            catch (UserProfileNotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
    }
}
