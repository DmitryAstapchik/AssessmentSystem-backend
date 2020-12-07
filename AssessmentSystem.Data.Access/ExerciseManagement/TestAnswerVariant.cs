namespace AssessmentSystem.Data.Access.ExerciseManagement
{
    /// <summary>
    /// Represents a variant of an answer to the test question.
    /// </summary>
    public class TestAnswerVariant
    {
        /// <summary>
        /// Gets or sets an id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a text of an answer.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsCorrect
        /// </summary>
        public bool IsCorrect { get; set; }

        /// <summary>
        /// Gets or sets the Question
        /// </summary>
        public TestQuestion Question { get; set; }

        /// <summary>
        /// Gets or sets the QuestionId
        /// </summary>
        public int QuestionId { get; set; }
    }
}
