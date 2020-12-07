using AssessmentSystem.Data.Access.Context;
using AssessmentSystem.Data.Access.Context.Interfaces;
using Ninject.Modules;

namespace AssessmentSystem.Data.Access
{
    /// <summary>
    /// Represents a registration module for Ninject.
    /// </summary>
    public class RegistrationModule : NinjectModule
    {
        /// <summary>
        /// Loads the module into the kernel.
        /// </summary>
        public override void Load()
        {
            Bind<ApplicationDbContext>().To<ApplicationDbContext>();
            Bind<IInvitesContext>().To<InvitesContext>();
            Bind<IExercisesContext>().To<ExercisesContext>();
            Bind<ITestAssemblyContext>().To<TestAssemblyContext>();
        }
    }
}
