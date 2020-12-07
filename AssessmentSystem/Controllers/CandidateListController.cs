using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using AssessmentSystem.Filters;
using AssessmentSystem.Models;
using AssessmentSystem.Models.CandidateManagement;
using AssessmentSystem.Services.ExerciseExecutor;
using AssessmentSystem.Services.Identity;
using AssessmentSystem.Services.Security;
using Swashbuckle.Swagger.Annotations;

namespace AssessmentSystem.Controllers
{
    /// <summary>
    /// Represents a controller for candidates management.
    /// </summary>
    [RoutePrefix("api/v1/candidates")]
    public class CandidateListController : ApiController
    {
        private readonly ApplicationUserManager _userManager;
        private readonly ICandidateExercisesResultsService _exercisesResultsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CandidateListController"/> class.
        /// </summary>
        /// <param name="userManager">An instance of <see cref="ApplicationUserManager"/>.</param>
        /// <param name="resultService">An implementation of <see cref="ICandidateExercisesResultsService"/>.</param>
        public CandidateListController(ApplicationUserManager userManager, ICandidateExercisesResultsService resultService)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _exercisesResultsService = resultService ?? throw new ArgumentNullException(nameof(resultService));
        }

        /// <summary>
        /// Gets candidates with their scores.
        /// </summary>
        /// <param name="parameters">Pagination parameters.</param>
        /// <param name="lastNamePart">Part of a lastname of a user to search users by lastname.</param>
        /// <param name="sortCriteria">Criteria of user sorting.</param>
        /// <param name="isDescending">Determines whether sorting is descending.</param>
        /// <returns>
        /// Candidates info with scores.
        /// </returns>
        /// <remarks>By default, a page index is 1, an amount is 25.</remarks>
        /// <exception cref="HttpResponseException">Page index is less or equals to zero or amount is less or equals to zero.</exception>
        // GET /api/v1/candidates
        // GET /api/v1/candidates?page=1&amount=25
        // GET /api/v1/candidates?page=1&amount=25&sortCriteria='email'&isDescending='false'
        [HttpGet]
        [Route("")]
        [ModelStateValidationFilter]
        [ResponseType(typeof(Candidate[]))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Parameter values are not correct")]
        public IHttpActionResult GetCandidates(
            [FromUri(Name = "")] PaginationParameters parameters,
            [FromUri(Name = "lastNamePart")] string lastNamePart = null,
            [FromUri(Name = "sortCriteria")] string sortCriteria = nameof(Candidate.LastName),
            [FromUri(Name = "isDescending")] bool isDescending = false)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            parameters = parameters ?? new PaginationParameters();
            try
            {
                var profiles = GetProfiles(
                    (parameters.Page - 1) * parameters.Amount,
                    parameters.Amount,
                    lastNamePart,
                    sortCriteria,
                    isDescending);

                var candidates = GetCandidates(profiles);
                return Ok(candidates.ToArray());
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        /// <summary>
        /// Gets a count of candidates.
        /// </summary>
        /// <param name="lastNamePart">Part of a lastname of a user to search users by lastname.</param>
        /// <returns>
        /// Count of candidates.
        /// </returns>
        // GET /api/v1/candidates/count
        // GET /api/v1/candidates/count?lastNamePart='Smith'
        [HttpGet]
        [Route("count")]
        [ResponseType(typeof(int))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error")]
        public IHttpActionResult GetCandidatesCount([FromUri(Name = "lastNamePart")] string lastNamePart = null)
        {
            try
            {
                var accountsCount = _userManager.GetAccountsCount(GlobalInfo.Candidate, lastNamePart);
                return Ok(accountsCount);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        private IEnumerable<Candidate> GetCandidates(IEnumerable<UserProfileInfo> profiles)
        {
            return from profile in profiles
                let results = _exercisesResultsService.GetCandidateExercisesResults(profile.Id)
                let score = results.Sum(result => result.Score)
                let maxScore = results.Sum(result => result.CandidateExercise.MaximumScore)
                select new Candidate
                {
                    Id = profile.Id,
                    FirstName = profile.FirstName,
                    LastName = profile.LastName,
                    Score = score,
                    MaximumScore = maxScore
                };
        }

        private IEnumerable<UserProfileInfo> SortByScore(IEnumerable<UserProfileInfo> profiles, bool isDescending)
        {
            return isDescending
                ? profiles.OrderByDescending(GetScore)
                : profiles.OrderBy(GetScore);
        }

        private string GetScore(UserProfileInfo profile)
        {
            var results = _exercisesResultsService.GetCandidateExercisesResults(profile.Id).ToArray();
            var score = results.Sum(result => result.Score);
            var maxScore = results.Sum(result => result.CandidateExercise.MaximumScore);
            return string.Concat(score, "/", maxScore);
        }

        private IEnumerable<UserProfileInfo> GetProfiles(
            int page,
            int amount,
            string lastNamePart,
            string sortCriteria,
            bool isDescending)
        {
            IEnumerable<UserProfileInfo> profiles;

            if (sortCriteria.Equals(nameof(Candidate.Score), StringComparison.OrdinalIgnoreCase))
            {
                profiles = _userManager.GetProfiles(
                    0,
                    _userManager.GetAccountsCount(GlobalInfo.Candidate),
                    GlobalInfo.Candidate,
                    lastNamePart);

                profiles = SortByScore(profiles, isDescending)
                    .Skip(page)
                    .Take(amount);
            }
            else
            {
                profiles = _userManager.GetProfiles(
                    page,
                    amount,
                    GlobalInfo.Candidate,
                    lastNamePart,
                    sortCriteria,
                    isDescending);
            }

            return profiles;
        }
    }
}