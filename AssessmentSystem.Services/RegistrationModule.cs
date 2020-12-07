using AssessmentSystem.Services.ExerciseExecutor;
using AssessmentSystem.Services.ExerciseManagement;
using AssessmentSystem.Services.UserManagement;
using Ninject.Modules;

namespace AssessmentSystem.Services
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
            Bind<ICandidateTaskEvaluationService>().To<CandidateTaskEvaluationService>();
            Bind<ICandidateExercisesResultsService>().To<CandidateExercisesResultsService>();
            Bind<ICandidateTaskRunner>().To<CandidateTaskRunner>();
            Bind<ITestAssemblyService>().To<TestAssemblyService>();
            Bind<IInvitesService>().To<InvitesService>();
            Bind<IEmailSendingService>().To<EmailSendingService>();
            Bind<IExerciseService>().To<ExerciseService>();
            Bind<ExerciseManagement.IExercisePoolService>().To<ExerciseManagement.ExercisePoolService>();
        }
    }
}
