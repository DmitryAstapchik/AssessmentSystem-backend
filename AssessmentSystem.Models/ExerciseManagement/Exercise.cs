namespace AssessmentSystem.Models.ExerciseManagement
{
    /// <summary>
    /// Represents an exam exercise.
    /// Exercise can be a task or a test.
    /// </summary>
    public class Exercise
    {
        /// <summary>
        /// Gets or sets an id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///  Gets or sets a title of the exercise.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets an exercise subject.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        ///  Gets or sets gets a full description of the exercise.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets maximum score per exercise.
        /// Determines the complexity of the exercises.
        /// </summary>
        /// <remarks>
        /// The higher the maximum score for the task, the more complexity it is.
        /// </remarks>
        public int MaximumScore { get; set; }

        /// <summary>
        /// Gets or sets exercise time in seconds.
        /// </summary>
        public long? TimeMinutes { get; set; }
    }
}