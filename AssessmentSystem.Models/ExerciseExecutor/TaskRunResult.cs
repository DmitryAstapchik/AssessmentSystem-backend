namespace AssessmentSystem.Models.ExerciseExecutor
{
    /// <summary>
    /// Represents a result of a task that was checked.
    /// </summary>
    public class TaskRunResult
    {
        /// <summary>
        /// Gets or sets a value indicating whether
        /// the candidate fulfilled the task successfully.
        /// </summary>
        /// <remarks>True if successfully, false otherwise.</remarks>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets an error message if the code was built with errors.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets a candidate's score from the task.
        /// </summary>
        public int Score { get; set; }
    }
}
