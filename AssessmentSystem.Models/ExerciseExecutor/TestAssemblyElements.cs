using System.Collections.Generic;

namespace AssessmentSystem.Models.ExerciseExecutor
{
    /// <summary>
    /// Represents a test assembly elements.
    /// </summary>
    public class TestAssemblyElements
    {
        /// <summary>
        /// Gets or sets an assembly name.
        /// </summary>
        public string AssemblyName { get; set; }

        /// <summary>
        /// Gets or sets assembly classes elements.
        /// </summary>
        public IEnumerable<TestClassElements> TestClassesElements { get; set; }
    }
}
