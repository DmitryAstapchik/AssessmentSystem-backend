using System.Collections.Generic;

namespace AssessmentSystem.Data.Access.ExerciseManagement
{
    /// <summary>
    /// Represents a test question.
    /// </summary>
    public class TestQuestion
    {
        /// <summary>
        /// Gets or sets an id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a candidate test to which the question belongs.
        /// </summary>
        public Test Test { get; set; }

        /// <summary>
        /// Gets or sets an candidate test id.
        /// </summary>
        public int TestId { get; set; }

        /// <summary>
        /// Gets or sets a full text of the question.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets test answers.
        /// </summary>
        public ICollection<TestAnswerVariant> AnswerVariants { get; set; }
    }
}
