using System;

namespace AssessmentSystem.Data.Access.ExerciseExecutor
{
    /// <summary>
    /// Represents an exam task result.
    /// </summary>
    public class TaskResult : ExerciseResult
    {
        /// <summary>
        /// Gets or sets candidate task code.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets a number of used tips.
        /// </summary>
        public int UsedTipsNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether task is comleted.
        /// </summary>
        public bool IsCompleted { get; set; }

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