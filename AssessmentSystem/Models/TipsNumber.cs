namespace AssessmentSystem.Models
{
    /// <summary>
    /// Represents a number of tips used by a candidate.
    /// </summary>
    public class TipsNumber
    {
        /// <summary>
        /// Gets or sets an id of a task that was made by a candidate.
        /// </summary>
        public int TaskId { get; set; }

        /// <summary>
        /// Gets or sets a number of tips used by a candidate.
        /// </summary>
        public int UsedTipsNumber { get; set; }
    }
}