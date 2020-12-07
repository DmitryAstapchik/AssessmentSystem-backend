using System.Collections.Generic;
using AssessmentSystem.Models.ExerciseManagement;

namespace AssessmentSystem.Services.ExerciseManagement
{
    /// <summary>
    /// Represents an exercise pool service contract.
    /// </summary>
    public interface IExercisePoolService
    {
        /// <summary>
        /// Returns a set of exercises that is assigned to the candidate by coach.
        /// </summary>
        /// <returns>Set of <see cref="Exercise"/>.</returns>
        IEnumerable<Exercise> GetActiveExerciseSet();

        /// <summary>
        /// Returns a task by id from active set.
        /// </summary>
        /// <param name="id">Identifier of the target task.</param>
        /// <returns>Instance of <see cref="Task"/>.</returns>
        Task GetCandidateTask(int id);
    }
}
