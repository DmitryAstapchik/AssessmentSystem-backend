using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AssessmentSystem.Models;
using AssessmentSystem.Models.ExerciseExecutor;
using AssessmentSystem.Services.Exceptions;
using AssessmentSystem.Services.ExerciseExecutor;
using AssessmentSystem.Services.Security;
using Swashbuckle.Swagger.Annotations;

namespace AssessmentSystem.Controllers
{
    /// <summary>
    /// Represents a controller that is responsible for a task evaluation.
    /// </summary>
    [RoutePrefix("api/v1/taskResults")]
    public class TaskResultsController : ApiController
    {
        private readonly ICandidateExercisesResultsService _exercisesResultsServise;
        private readonly ICandidateTaskEvaluationService _taskEvaluationService;
        private readonly IUserContextService _userContextService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskResultsController"/> class.
        /// </summary>
        /// <param name="candidateExercisesResultsServise">A candidate exercise servise.</param>
        /// <param name="service">A candidate task evaluation service.</param>
        /// <param name="userContextService">A user context service.</param>
        public TaskResultsController(ICandidateExercisesResultsService candidateExercisesResultsServise, ICandidateTaskEvaluationService service, IUserContextService userContextService)
        {
            _exercisesResultsServise = candidateExercisesResultsServise ?? throw new ArgumentNullException(nameof(candidateExercisesResultsServise));

            _taskEvaluationService = service ?? throw new ArgumentNullException(nameof(service));

            _userContextService = userContextService ?? throw new ArgumentNullException(nameof(userContextService));
        }

        /// <summary>
        /// Gets a andidate's task result found by id.
        /// </summary>
        /// <param name="id">A candidate's task result ID.</param>
        /// <returns>A candidate's task result.</returns>
        // [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("{id:int:min(1)}", Name = nameof(GetCandidateTaskResultById))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Parameter values are not correct")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "A task result with this ID doesn't exist")]
        [ResponseType(typeof(Models.ExerciseExecutor.CandidateTaskResult))]
        [Authorize(Roles = GlobalInfo.Candidate)]
        public IHttpActionResult GetCandidateTaskResultById(int id)
        {
            if (id > 0)
            {
                try
                {
                    var user = _userContextService.GetCurrentUser();
                    var candidateTaskResult = _exercisesResultsServise.GetTaskResultById(id, user.Id);
                    return Ok(candidateTaskResult);
                }
                catch (Exception)
                {
                    return InternalServerError();
                }
            }

            return InternalServerError();
        }

        /// <summary>
        /// Gets a candidate's task result by task id.
        /// </summary>
        /// <param name="taskId">A candidate's task ID.</param>
        /// <returns>A candidate's task result.</returns>
        [HttpGet]
        [Route("task/{taskId:int:min(1)}")]
        [Authorize(Roles = GlobalInfo.Candidate)]
        [ResponseType(typeof(Models.ExerciseExecutor.CandidateTaskResult))]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "A task result with this ID doesn't exist")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "User is not authorized.")]
        public IHttpActionResult GetCandidateTaskResultByTaskId(int taskId)
        {
            try
            {
                var user = _userContextService.GetCurrentUser();
                var candidateTaskResult = _exercisesResultsServise.GetTaskResultByTaskId(taskId, user.Id);
                if (candidateTaskResult == null)
                {
                    return NotFound();
                }

                return Ok(candidateTaskResult);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [Route("{id:int:min(1)}/{idCandidate}")]
        [Authorize(Roles = GlobalInfo.Manager)]
        public IHttpActionResult GetCandidateTaskResultById(int id, Guid idCandidate)
        {
            if (id > 0)
            {
                try
                {
                    var candidateTaskResult = _exercisesResultsServise.GetTaskResultById(id, idCandidate);
                    return Ok(candidateTaskResult);
                }
                catch (Exception)
                {
                    return InternalServerError();
                }
            }

            return InternalServerError();
        }

        /// <summary>
        /// Create new candidate task result.
        /// </summary>
        /// <param name="taskId">A candidate task id.</param>
        /// <returns>Result of creation new candidate task.</returns>
        [HttpPost]
        [Route("create")]
        [Authorize]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Task result already exists.")]
        [SwaggerResponse(HttpStatusCode.Created, Description = "Task result cerated successfully.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "User is not authorized.")]
        public async Task<IHttpActionResult> CreateCandidateTaskResult(int taskId)
        {
            try
            {
                var user = _userContextService.GetCurrentUser();
                await _taskEvaluationService.CreateCandidateTaskResultAsync(taskId, DateTime.UtcNow, user.Id);
                var actionLink = Url.Link(nameof(GetCandidateTaskResultById), new { id = taskId });
                return Created(actionLink, new { id = taskId });
            }
            catch (TaskResultAlreadyExistsException)
            {
                return BadRequest("Task result already exists.");
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        /// <summary>
        /// Gets all candidate's task results.
        /// </summary>
        /// <returns>An IEnumerable of task results.</returns>
        [Route("")]
        [HttpGet]
        [Authorize]
        [ResponseType(typeof(IEnumerable<Models.ExerciseExecutor.CandidateTaskResult>))]
        public IHttpActionResult GetCandidateTasksResults()
        {
            try
            {
                var user = _userContextService.GetCurrentUser();
                var candidateTaskResults = _exercisesResultsServise.GetTaskResults(user.Id);
                return Ok(candidateTaskResults);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        /// <summary>
        /// Validates a candidate's code.
        /// </summary>
        /// <param name="taskResult">Task result.</param>
        /// <returns>Result of a validation.</returns>
        [HttpPost]
        [Authorize]
        [Route("")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Parameter values are not correct")]
        [ResponseType(typeof(TaskRunResult))]
        public async Task<IHttpActionResult> ValidateTask([FromBody]TaskResult taskResult)
        {
            if (!ModelState.IsValid)
            {
                BadRequest(ModelState);
            }

            var user = await _userContextService.GetCurrentUserAsync();

            try
            {
                var result = await _taskEvaluationService.ValidateAsync(
                    taskResult.TaskId,
                    taskResult.Template,
                    taskResult.UsedTipsNumber,
                    user.Id);

                return Ok(result);
            }
            catch (TimerOutException)
            {
                return BadRequest("Timer went out.");
            }
        }

        [HttpPost]
        [Route("addTip")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Parameter values are not correct")]
        public async Task<IHttpActionResult> UploadCurrentTipsNumber([FromBody] TipsNumber tipsNUmber)
        {
            try
            {
                var user = await _userContextService.GetCurrentUserAsync();
                await _taskEvaluationService.UploadTipsNumber(tipsNUmber.TaskId, tipsNUmber.UsedTipsNumber, user.Id);
            }
            catch (Exception)
            {
                return InternalServerError();
            }

            return Ok();
        }

        /// <summary>
        /// Deletes a candidate's task result by it's ID.
        /// </summary>
        /// <param name="id">ID of a candidate's task result.</param>
        /// <returns>A deleting operation result.</returns>
        [HttpDelete]
        [Authorize(Roles = GlobalInfo.Manager)]
        [Route("{id:int:min(1)}")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Parameter values are not correct")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error")]
        public async Task<IHttpActionResult> Delete([FromUri]int id)
        {
            try
            {
                await _exercisesResultsServise.RemoveTaskResultAsync(id);
                return Ok();
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
    }
}
