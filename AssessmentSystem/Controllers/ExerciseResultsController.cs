using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using AssessmentSystem.Models.ExerciseExecutor;
using AssessmentSystem.Services.ExerciseExecutor;
using Swashbuckle.Swagger.Annotations;

namespace AssessmentSystem.Controllers
{
    /// <summary>
    /// Represents a controller for actions with candidate exercise results.
    /// </summary>
    [RoutePrefix("api/v1/exerciseResults")]
    [Authorize(Roles = GlobalInfo.Manager)]
    public class ExerciseResultsController : ApiController
    {
        private readonly ICandidateExercisesResultsService _exercisesResultsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExerciseResultsController"/> class.
        /// </summary>
        /// <param name="exercisesResultsService">An instance of the exercise results service.</param>
        public ExerciseResultsController(ICandidateExercisesResultsService exercisesResultsService)
        {
            _exercisesResultsService = exercisesResultsService ?? throw new ArgumentNullException(nameof(exercisesResultsService));
        }

        /// <summary>
        /// Gets candidate's completed exercises results.
        /// </summary>
        /// <param name="candidateId">Candidate's domain id.</param>
        /// <returns>Candidate's completed exercises results.</returns>
        // GET api/v1/exerciseResults
        [Route("{candidateId:guid}")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No candidate's results are found")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error")]
        [ResponseType(typeof(IEnumerable<CandidateExerciseResult>))]
        public IHttpActionResult GetCandidateExcerciseResults(Guid candidateId)
        {
            try
            {
                var results = _exercisesResultsService.GetCandidateExercisesResults(candidateId);
                if (results.Count() == 0)
                {
                    return NotFound();
                }

                return Ok(results);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
    }
}