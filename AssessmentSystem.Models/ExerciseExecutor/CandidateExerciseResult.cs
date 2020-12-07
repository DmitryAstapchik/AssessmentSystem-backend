using System;
using AssessmentSystem.Models.ExerciseManagement;

namespace AssessmentSystem.Models.ExerciseExecutor
{
    /// <summary>
    /// Represents the exam exercise result.
    /// </summary>
    public class CandidateExerciseResult
    {
        /// <summary>
        /// Gets or sets an id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a candidate exercise
        /// </summary>
        public Exercise CandidateExercise { get; set; }

        /// <summary>
        /// Gets or sets a candidate exercise id.
        /// </summary>
        public int CandidateExerciseId { get; set; }

        /// <summary>
        /// Gets or sets score that the candidate received for completing the exercise.
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// Gets or sets ID of task creator.
        /// </summary>
        public Guid CreatorId { get; set; }

        /// <summary>
        /// Gets or sets ID of task modifier.
        /// </summary>
        public Guid ModifierId { get; set; }
    }
}