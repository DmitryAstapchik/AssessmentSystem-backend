using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace AssessmentSystem.Filters
{
    /// <summary>
    /// Represents an <see cref="ActionFilterAttribute"/> for <see cref="ModelStateDictionary."/>
    /// </summary>
    public class ModelStateValidationFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var modelState = actionExecutedContext.ActionContext.ModelState;

            if (modelState.IsValid == false)
            {
                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.BadRequest, new ValidationErrorResponse
                {
                    Messages = modelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToArray()
                });
            }
            else
            {
                base.OnActionExecuted(actionExecutedContext);
            }
        }
    }
}
