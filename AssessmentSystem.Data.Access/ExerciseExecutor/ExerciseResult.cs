using System;
using System.ComponentModel.DataAnnotations;
using AssessmentSystem.Data.Access.ExerciseManagement;

namespace AssessmentSystem.Data.Access.ExerciseExecutor
{
    /// <summary>
    /// Represents an exam exercise result.
    /// </summary>
    public class ExerciseResult
    {
        /// <summary>
        /// Gets or sets an id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a candidate exercise.
        /// </summary>
        public virtual Exercise CandidateExercise { get; set; }

        /// <summary>
        /// Gets or sets a candidate exercise id.
        /// </summary>
        public int CandidateExerciseId { get; set; }

        /// <summary>
        /// Gets or sets a score that a candidate received for completing an exercise.
        /// </summary>
        [Required]
        public int Score { get; set; }

        [Required]
        public Guid CreatorId { get; set; }

        [Required]
        public Guid ModifierId { get; set; }
    }
}