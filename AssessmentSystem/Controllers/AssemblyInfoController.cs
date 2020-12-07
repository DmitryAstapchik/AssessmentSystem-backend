using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AssessmentSystem.Filters;
using AssessmentSystem.Models.ExerciseExecutor;
using AssessmentSystem.Services.Exceptions;
using AssessmentSystem.Services.ExerciseExecutor;
using Swashbuckle.Swagger.Annotations;

namespace AssessmentSystem.Controllers
{
    /// <summary>
    /// Represents a controller that is responsible for test assemblies.
    /// </summary>
    // [Authorize(Roles = "admin")]
    [RoutePrefix("api/v1/assemblyInfo")]
    public class AssemblyInfoController : ApiController
    {
        private readonly ITestAssemblyService _testAssembliesService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyInfoController"/> class.
        /// </summary>
        /// <param name="testAssembliesService">A test assembly service.</param>
        /// <exception cref="ArgumentNullException">Exception thrown when
        /// <paramref name="testAssembliesService"/> is null.</exception>
        public AssemblyInfoController(ITestAssemblyService testAssembliesService)
        {
            _testAssembliesService = testAssembliesService ?? throw new ArgumentNullException(nameof(testAssembliesService));
        }

        /// <summary>
        /// Gets an assembly found by id.
        /// </summary>
        /// <param name="id">An id of an assembly.</param>
        /// <returns>An assembly found by id.</returns>
        [HttpGet]
        [Route("{id:int:min(1)}", Name = nameof(GetAssemblyById))]
        [ResponseType(typeof(TestAssemblyElements))]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Assembly not found")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Parameter value is not correct")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error")]
        public IHttpActionResult GetAssemblyById(int id)
        {
            try
            {
                var assembly = _testAssembliesService.GetTestAssembly(id);
                if (assembly == null)
                {
                    return NotFound();
                }

                return Ok(assembly);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        /// <summary>
        /// Deletes a test assembly by id.
        /// </summary>
        /// <param name="id">An id of an assembly.</param>
        /// <returns>A deleting operation result.</returns>
        [HttpDelete]
        [Route("{id:int:min(1)}")]
        [ResponseType(typeof(void))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Parameter value is not correct")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error")]
        public async Task<IHttpActionResult> DeleteAssembly(int id)
        {
            try
            {
                if (_testAssembliesService.IsTaskAssociatedWithAssemblyExist(id))
                {
                    await _testAssembliesService.SoftRemoveTestAssemblyAsync(id);
                }
                else
                {
                    await _testAssembliesService.RemoveTestAssemblyAsync(id);
                }

                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (TestAssemblyNotFoundException)
            {
                return BadRequest($"Assembly with {nameof(id)} = {id} not found");
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        /// <summary>
        /// Uploads an assembly.
        /// </summary>
        /// <returns>Created assemby id.</returns>
        [HttpPost]
        [Route("")]
        [SwaggerOperation("apiAssemblyInfoControllerPostAssemblypost")]
        [SwaggerOperationFilter(typeof(FileUploadOperationFilter))]
        [SwaggerResponse(HttpStatusCode.Created, Description = "A new assembly is created")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Parameter value is not correct")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error")]
        public async Task<IHttpActionResult> CreateAssembly()
        {
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                {
                    return StatusCode(HttpStatusCode.UnsupportedMediaType);
                }

                var filesToReadProvider = await Request.Content.ReadAsMultipartAsync();
                var stream = filesToReadProvider.Contents[0];
                byte[] assemblyBytes = await stream.ReadAsByteArrayAsync();

                int createdAssemblyId = await _testAssembliesService.AddTestAssemblyAsync(assemblyBytes);

                var actionLink = Url.Link(nameof(GetAssemblyById), new { id = createdAssemblyId });
                return Created(actionLink, new { Id = createdAssemblyId });
            }
            catch (BadImageFormatException)
            {
                return BadRequest("Raw assembly data is not a valid assembly");
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        /// <summary>
        /// Returns a list of assembly elements.
        /// </summary>
        /// <returns>Assembly elements list.</returns>
        [HttpGet]
        [Route("elements")]
        [ResponseType(typeof(IEnumerable<TestAssemblyElements>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error")]
        public IHttpActionResult GetAssembliesElements()
        {
            try
            {
                var assembliesElements = _testAssembliesService.GetTestAssembliesElements();
                return Ok(assembliesElements.OrderBy(_ => _.AssemblyName).ToArray());
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        /// <summary>
        /// Returns a list of assemblies names and ids.
        /// </summary>
        /// <remarks>
        /// Item1 is the name of the assembly, item2 is the id of the assembly.
        /// </remarks>
        /// <returns>A list of assemblies names and ids.</returns>
        [HttpGet]
        [Route("names-and-ids")]
        [ResponseType(typeof(IEnumerable<Tuple<string, int>>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error")]
        public IHttpActionResult GetExercisesNamesAndIds()
        {
            try
            {
                var assembliesNamesAndIds = _testAssembliesService.GetAssembliesNamesAndIds();
                return Ok(assembliesNamesAndIds.OrderBy(_ => _.Item2).ToArray());
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
    }
}
