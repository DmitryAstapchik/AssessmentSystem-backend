namespace AssessmentSystem.Data.Access.ExerciseManagement
{
    /// <summary>
    /// Represents an exam task tip.
    /// </summary>
    public class TaskTip
    {
        /// <summary>
        /// Gets or sets an id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a tip text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets a candidate task id.
        /// </summary>
        public int CandidateTaskId { get; set; }

        /// <summary>
        /// Gets or sets a candidate task.
        /// </summary>
        public virtual Task CandidateTask { get; set; }
    }
}
