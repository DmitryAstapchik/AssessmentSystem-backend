namespace AssessmentSystem.Models.ExerciseManagement
{
    /// <summary>
    /// Represents an answer to a test question.
    /// </summary>
    public class TestAnswerVariant
    {
        /// <summary>
        /// Gets or sets id of an answer.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a text of the answer.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the answer is correct.
        /// </summary>
        public bool IsCorrect { get; set; }
    }
}
