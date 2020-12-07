using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AssessmentSystem.Models.ExerciseManagement;
using AssessmentSystem.Services.Exceptions;
using AssessmentSystem.Services.ExerciseManagement;
using AssessmentSystem.Services.Security;
using Swashbuckle.Swagger.Annotations;

namespace AssessmentSystem.Controllers
{
    /// <summary>
    /// Represents controller that is responsible for exercise management.
    /// </summary>
    // [Authorize]
    [RoutePrefix("api/v1/exercises")]
    public class ExercisesController : ApiController
    {
        private readonly IExerciseService _exerciseService;
        private readonly IUserContextService _userContextService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExercisesController"/> class.
        /// </summary>
        /// <param name="exerciseService">Exercise service.</param>
        /// <param name="userContextService">User context service.</param>
        /// <exception cref="ArgumentNullException">Exception thrown when
        /// <paramref name="exerciseService"/> or <paramref name="userContextService"/> is null.</exception>
        public ExercisesController(IExerciseService exerciseService, IUserContextService userContextService)
        {
            _exerciseService = exerciseService ?? throw new ArgumentNullException(nameof(exerciseService));
            _userContextService = userContextService ?? throw new ArgumentNullException(nameof(userContextService));
        }

        /// <summary>
        /// Returns a list of active exercises for the candidate appointed by the coach.
        /// </summary>
        /// <returns>Active task set.</returns>
        [HttpGet]
        [Route("candidate")]
        [ResponseType(typeof(IEnumerable<ExerciseForList>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error")]
        public async Task<IHttpActionResult> GetCandidateExercises()
        {
            try
            {
                var userContext = await _userContextService.GetCurrentUserAsync();

                var models = _exerciseService.GetCandidateExerciseList(userContext.Id);
                var result = models.OrderBy(exercise => exercise.Name).ToArray();

                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [HttpGet]
        [Route("tests/info")]
        [ResponseType(typeof(IEnumerable<Exercise>))]
        public IHttpActionResult GetTestsInfo()
        {
            return Ok(_exerciseService.GetTestsInfo());
        }

        [HttpGet]
        [Route("tasks/info")]
        [ResponseType(typeof(IEnumerable<Exercise>))]
        public IHttpActionResult GetTasksInfo()
        {
            return Ok(_exerciseService.GetTasksInfo());
        }

        [HttpGet]
        [Route("tests/{id}")]
        [ResponseType(typeof(Test))]
        public IHttpActionResult GetTest(int id)
        {
            return Ok(_exerciseService.GetTest(id));
        }

        [HttpPut]
        [Route("tests/{id}")]
        [ResponseType(typeof(void))]
        public async System.Threading.Tasks.Task PutTestAsync(int id, Test test)
        {
            await _exerciseService.UpdateTestAsync(id, test);
        }

        /// <summary>
        /// Deletes a test by its id asynchronously.
        /// </summary>
        /// <param name="id">A test id.</param>
        /// <returns>A <see cref="System.Threading.Tasks.Task"/> of test deletion.</returns>
        [HttpDelete]
        [Route("tests/{id}")]
        [ResponseType(typeof(void))]
        public async System.Threading.Tasks.Task DeleteTestAsync(int id)
        {
            await _exerciseService.DeleteTestAsync(id);
        }

        [HttpPost]
        [Route("tests")]
        [ResponseType(typeof(IHttpActionResult))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error")]
        public async System.Threading.Tasks.Task<IHttpActionResult> CreateTestAsync(Test test)
        {
            await _exerciseService.AddTestAsync(test);
            return StatusCode(HttpStatusCode.Created);
        }

        /// <summary>
        /// Returns a candidate task by id from the exercises appointed by the coach.
        /// </summary>
        /// <param name="id">Identifier of the target candidate task.</param>
        /// <returns>Candidate task with specified id.</returns>
        [HttpGet]
        [Route("candidate/task/{id:int:min(1)}")]
        [ResponseType(typeof(TaskViewInfo))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Parameter value is not correct")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Candidate task not found")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error")]
        public async Task<IHttpActionResult> GetCandidateTaskViewInfo(int id)
        {
            try
            {
                var userContext = await _userContextService.GetCurrentUserAsync();
                var result = _exerciseService.GetCandidateTaskViewInfo(id, userContext.Id);
                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        /// <summary>
        /// Returns a candidate task info identifying by id.
        /// </summary>
        /// <param name="id">Identifier of the target candidate task.</param>
        /// <returns>A candidate task info with specified id.</returns>
        [HttpGet]
        [Route("task/{id:int:min(1)}", Name = nameof(GetCandidateTaskInfo))]
        [ResponseType(typeof(TaskInfo))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Parameter value is not correct")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Task info not found")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error")]
        public IHttpActionResult GetCandidateTaskInfo(int id)
        {
            try
            {
                var taskInfo = _exerciseService.GetCandidateTaskInfo(id);
                if (taskInfo == null)
                {
                    return NotFound();
                }

                return Ok(taskInfo);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        /// <summary>
        /// Returns a list of exercises names and ids.
        /// </summary>
        /// <remarks>
        /// Item1 is the name of the exercise, item2 is the id of the exercise.
        /// </remarks>
        /// <returns>A list of exercises names and ids.</returns>
        [HttpGet]
        [Route("names-and-ids")]
        [ResponseType(typeof(IEnumerable<Tuple<string, int>>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error")]
        public IHttpActionResult GetExercisesNamesAndIds()
        {
            try
            {
                var exercisesNamesAndIds = _exerciseService.GetExercisesNamesAndIds();
                return Ok(exercisesNamesAndIds.OrderBy(_ => _.Item2).ToArray());
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        /// <summary>
        /// Creates a new exam task.
        /// </summary>
        /// <param name="candidateTaskInfo">A task info.</param>
        /// <returns>Created task id.</returns>
        [HttpPost]
        [Route("create-task")]
        [SwaggerResponse(HttpStatusCode.Created, Description = "A new task is created")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Parameter values are not correct")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error")]
        public async Task<IHttpActionResult> CreateCandidateTask([FromBody]TaskInfo candidateTaskInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                int createdTaskId = await _exerciseService.AddCandidateTaskAsync(candidateTaskInfo);
                var actionLink = Url.Link(nameof(GetCandidateTaskInfo), new { id = createdTaskId });
                return Created(actionLink, new { Id = createdTaskId });
            }
            catch (TestClassInfoNotFoundException)
            {
                return BadRequest($"Invalid {nameof(TaskInfo.TestClassName)}");
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        /// <summary>
        /// Updates a task identifying by id.
        /// </summary>
        /// <param name="candidateTaskInfo">A new task info.</param>
        /// <returns>An updating operation result.</returns>
        [HttpPut]
        [Route("update-task")]
        [ResponseType(typeof(void))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Parameter values are not correct")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error")]
        public async Task<IHttpActionResult> UpdateCandidateTask([FromBody]TaskInfo candidateTaskInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _exerciseService.UpdateCandidateTaskAsync(candidateTaskInfo);
                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (TaskNotFoundException)
            {
                return BadRequest($"Task with {nameof(TaskInfo.Id)} = {candidateTaskInfo.Id} not found");
            }
            catch (TestClassInfoNotFoundException)
            {
                return BadRequest($"Invalid {nameof(TaskInfo.TestClassName)}");
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        /// <summary>
        /// Deletes an exercise identifying by id.
        /// </summary>
        /// <param name="id">An exercise id.</param>
        /// <returns>A deletion operation result.</returns>
        [HttpDelete]
        [Route("tasks/{id:int:min(1)}")]
        [ResponseType(typeof(void))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Parameter value is not correct")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error")]
        public async Task<IHttpActionResult> DeleteCandidateTask(int id)
        {
            try
            {
                var result = _exerciseService.IsResultAssociatedWithTaskExist(id);
                if (result == true)
                {
                    await _exerciseService.SoftDeleteCandidateExerciseAsync(id);
                }
                else
                {
                    await _exerciseService.DeleteCandidateExerciseAsync(id);
                }

                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (ExerciseNotFoundException)
            {
                return BadRequest($"Task with {nameof(id)} = {id} not found");
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
    }
}
