using System.Collections.Generic;

namespace AssessmentSystem.Models.ExerciseExecutor
{
    /// <summary>
    /// Represents a test class elements.
    /// </summary>
    public class TestClassElements
    {
        /// <summary>
        /// Gets or sets a class name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets class methods.
        /// </summary>
        public IEnumerable<string> MethodsNames { get; set; }
    }
}
