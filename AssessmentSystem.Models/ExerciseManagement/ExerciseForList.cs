namespace AssessmentSystem.Models.ExerciseManagement
{
    /// <summary>
    /// Represents an exercise list item.
    /// </summary>
    public class ExerciseForList
    {
        /// <summary>
        /// Gets or sets an id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets an exercise title.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets an exercise subject.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets an exercise maximum score.
        /// </summary>
        public int MaximumScore { get; set; }

        /// <summary>
        /// Gets or sets exercise time in seconds.
        /// </summary>
        public long? TimeSeconds { get; set; }

        /// <summary>
        /// Gets or sets the score that the candidate scored for completing the assignment.
        /// </summary>
        /// <remarks>-1 if the candidate did not complete the task.</remarks>
        public int CandidateScore { get; set; }

        /// <summary>
        /// Gets or sets a number of tips that the candidate used.
        /// </summary>
        /// <remarks>For the test this value is always 0.</remarks>
        public int UsedTipsNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the candidate completed the exercise.
        /// </summary>
        /// <remarks>True if completed, false otherwise.</remarks>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether an exercise is soft deleted.
        /// </summary>
        public bool IsSoftDeleted { get; set; }

        /// <summary>
        /// Gets or sets an id of candidate's result.
        /// </summary>
        public int ResultId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the timer is out.
        /// </summary>
        public bool IsTimeOut { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the exercise has timer.
        /// </summary>
        public bool HasTimer { get; set; }
    }
}