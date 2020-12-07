using System;
using System.Threading.Tasks;
using AssessmentSystem.Models.ExerciseExecutor;

namespace AssessmentSystem.Services.ExerciseExecutor
{
    /// <summary>
    /// Represents a candidate task evaluation service contract.
    /// </summary>
    public interface ICandidateTaskEvaluationService
    {
        /// <summary>
        /// Validates a candidate's code.
        /// </summary>
        /// <param name="taskId">An id of a task.</param>
        /// <param name="code">A code of a candidate.</param>
        /// <param name="usedTipsNumber">A number of tips used by a candidate.</param>
        /// <param name="userId">An id of a candidate.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<TaskRunResult> ValidateAsync(
            int taskId, string code, int usedTipsNumber, Guid userId);

        /// <summary>
        /// Uploads a number of tips used by a candidate.
        /// </summary>
        /// <param name="taskId">An id of a task.</param>
        /// <param name="usedTipsNumber">A number of used tips.</param>
        /// <param name="userId">An id of a candidate.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task UploadTipsNumber(int taskId, int usedTipsNumber, Guid userId);

        /// <summary>
        /// Creates candidate task result.
        /// </summary>
        /// <param name="taskId">An id of a task.</param>
        /// <param name="startDate">Task start date.</param>
        /// <param name="userId">An id of a candidate.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task CreateCandidateTaskResultAsync(int taskId, DateTime startDate, Guid userId);
    }
}
