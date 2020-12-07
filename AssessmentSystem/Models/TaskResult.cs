namespace AssessmentSystem.Models
{
    /// <summary>
    /// Represents a task result.
    /// </summary>
    public class TaskResult
    {
        /// <summary>
        /// Gets or sets an id of a task that was made by a candidate.
        /// </summary>
        public int TaskId { get; set; }

        /// <summary>
        /// Gets or sets a code that was sent by a candidate.
        /// </summary>
        public string Template { get; set; }

        /// <summary>
        /// Gets or sets a number of tips used by a candidate.
        /// </summary>
        public int UsedTipsNumber { get; set; }
    }
}