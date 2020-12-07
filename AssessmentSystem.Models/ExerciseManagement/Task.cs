using System.Collections.Generic;
using AssessmentSystem.Models.ExerciseExecutor;

namespace AssessmentSystem.Models.ExerciseManagement
{
    /// <summary>
    /// Represents an exam task.
    /// </summary>
    public class Task : Exercise
    {
        /// <summary>
        /// Gets or sets a code template.
        /// </summary>
        public string CodeTemplate { get; set; }

        /// <summary>
        /// Gets or sets a test method.
        /// </summary>
        public TestMethodInfo TestMethod { get; set; }

        /// <summary>
        /// Gets or sets a test method id.
        /// </summary>
        public int? TestMethodId { get; set; }

        /// <summary>
        /// Gets or sets a test class.
        /// </summary>
        public TestClassInfo TestClass { get; set; }

        /// <summary>
        /// Gets or sets a test class id.
        /// </summary>
        public int TestClassId { get; set; }

        /// <summary>
        /// Gets or sets tips for a particular exercise.
        /// </summary>
        public IEnumerable<string> Tips { get; set; }
    }
}
