using System.Web.Http.Dependencies;
using Ninject;
using ApiDependencyResolver = System.Web.Http.Dependencies.IDependencyResolver;
using MvcDependencyResolver = System.Web.Mvc.IDependencyResolver;

namespace AssessmentSystem.Infrastructure
{
    public class NInjectDependencyResolver : NInjectDependencyScope, MvcDependencyResolver, ApiDependencyResolver
    {
        private readonly IKernel _kernel;

        public NInjectDependencyResolver(IKernel kernel)
            : base(kernel)
        {
            _kernel = kernel;
        }

        public IDependencyScope BeginScope()
        {
            return new NInjectDependencyScope(_kernel.BeginBlock());
        }
    }
}
