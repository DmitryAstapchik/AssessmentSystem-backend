using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Http.Results;
using AssessmentSystem.Services.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace AssessmentSystem.Filters
{
    public class BasicAuthenticationFilter : Attribute, IAuthenticationFilter
    {
        public bool AllowMultiple => false;

        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            AuthenticationHeaderValue authentication = context.Request.Headers.Authorization;
            if (authentication != null && authentication.Scheme == "Basic")
            {
                var authData = Encoding.ASCII.GetString(Convert.FromBase64String(authentication.Parameter)).Split(':');
                var login = authData[0];
                var roles = HttpContext.Current
                    .GetOwinContext()
                    .Get<ApplicationUserManager>()
                    .GetUserRolesByCredentials(login, authData[1])
                    .ToArray();

                context.Principal = roles.Length == 0 ? null : new GenericPrincipal(new GenericIdentity(login), roles);

                if (context.Principal == null)
                {
                    context.ErrorResult
                        = new UnauthorizedResult(new[] { new AuthenticationHeaderValue("Basic") }, context.Request);
                }
            }

            return Task.FromResult<object>(null);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult<object>(null);
        }
    }
}