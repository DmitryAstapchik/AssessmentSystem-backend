using System;

namespace AssessmentSystem.Models.ExerciseExecutor
{
    /// <summary>
    /// Represents candidate's result of a fulfilled task.
    /// </summary>
    public class CandidateTaskResult : CandidateExerciseResult
    {
        /// <summary>
        /// Gets or sets candidate task code.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the number of used tips.
        /// </summary>
        public int UsedTipsNumber { get; set; }

        /// <summary>
        /// Gets or sets a task start date.
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets a task end date.
        /// </summary>
        public DateTime? EndDate { get; set; }
    }
}