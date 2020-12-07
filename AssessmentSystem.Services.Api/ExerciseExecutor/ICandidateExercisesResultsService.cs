using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AssessmentSystem.Models.ExerciseExecutor;

namespace AssessmentSystem.Services.ExerciseExecutor
{
    /// <summary>
    /// Represents an exercise service contract.
    /// </summary>
    public interface ICandidateExercisesResultsService
    {
        /// <summary>
        /// Returns all candidate's task results.
        /// </summary>
        /// <param name="candidateId">Candidate's domain id.</param>
        /// <returns>All task results.</returns>
        IEnumerable<CandidateTaskResult> GetTaskResults(Guid candidateId);

        /// <summary>
        /// Finds a task result by result id and candidate id.
        /// </summary>
        /// <param name="resultId">An id of a result.</param>
        /// /// <param name="candidateId">A candidate id.</param>
        /// <returns>An instance of <see cref="CandidateTaskResult"/>.</returns>
        CandidateTaskResult GetTaskResultById(int resultId, Guid candidateId);

        /// <summary>
        /// Removes a task result by id.
        /// </summary>
        /// <param name="id">An id of a task result.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task RemoveTaskResultAsync(int id);

        /// <summary>
        /// Finds a task result by task id and candidate id.
        /// </summary>
        /// <param name="taskId">An id of a task.</param>
        /// <param name="candidateId">A candidate id.</param>
        /// <returns>An instance of <see cref="CandidateTaskResult"/>.</returns>
        CandidateTaskResult GetTaskResultByTaskId(int taskId, Guid candidateId);

        /// <summary>
        /// Gets candidate's completed exercises results.
        /// </summary>
        /// <param name="candidateId">Candidate's domain id.</param>
        /// <returns>Candidate's completed exercises results.</returns>
        IEnumerable<CandidateExerciseResult> GetCandidateExercisesResults(Guid candidateId);
    }
}
