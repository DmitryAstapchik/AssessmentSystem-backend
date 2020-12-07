using Ninject.Modules;

namespace AssessmentSystem.Runner
{
    /// <summary>
    /// Represents a registration module for NInject.
    /// </summary>
    public class RegistrationModule : NinjectModule
    {
        /// <summary>
        /// Loads the module into the kernel.
        /// </summary>
        public override void Load()
        {
            Bind<INUnitFilterFactory>().To<NUnitFilterFactory>();
            Bind<UserTestRunner>().To<UserTestRunner>();
        }
    }
}
