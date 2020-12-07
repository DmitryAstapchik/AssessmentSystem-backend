using System.Collections.Generic;

namespace AssessmentSystem.Data.Access.ExerciseManagement
{
    /// <summary>
    /// Represents an exam candidate test.
    /// </summary>
    public class Test : Exercise
    {
        /// <summary>
        /// Gets or sets test questions.
        /// </summary>
        public ICollection<TestQuestion> Questions { get; set; }
    }
}
