using AssessmentSystem.Data.Access.ExerciseExecutor;
using AssessmentSystem.Data.Access.ExerciseManagement;

namespace AssessmentSystem.Data.Access.Context.Interfaces
{
    /// <summary>
    /// Represents an exercise context.
    /// </summary>
    public interface IExercisesContext
    {
        /// <summary>
        /// Gets a candidate tasks set.
        /// </summary>
        IEntitySet<ExerciseManagement.Task> CandidateTasks { get; }

        /// <summary>
        /// Gets a candidate tests set.
        /// </summary>
        IEntitySet<Test> CandidateTests { get; }

        IEntitySet<TestQuestion> TestQuestions { get; }

        IEntitySet<TestAnswerVariant> TestAnswerVariants { get; }

        /// <summary>
        /// Gets a candidate tasks results set.
        /// </summary>
        IEntitySet<TaskResult> CandidateTaskResults { get; }

        /// <summary>
        /// Gets a candidate tests results set.
        /// </summary>
        IEntitySet<TestResult> CandidateTestResults { get; }

        /// <summary>
        /// Gets a candidate tasks tips set.
        /// </summary>
        IEntitySet<TaskTip> CandidateTaskTips { get; }

        /// <summary>
        /// Saves all changes made in this context to an underlying storage.
        /// </summary>
        /// <returns>A task result of saving to a data base.</returns>
        System.Threading.Tasks.Task SaveChangesAsync();
    }
}
