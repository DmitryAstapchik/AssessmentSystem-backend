using System.Collections.Generic;

namespace AssessmentSystem.Models.ExerciseExecutor
{
    /// <summary>
    /// Represent a class for testing a candidate's task.
    /// </summary>
    public class TestClassInfo
    {
        /// <summary>
        /// Gets or sets an id of a test class.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a name of a test class.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a test assembly which this class belongs to.
        /// </summary>
        public TestAssemblyInfo AssemblyInfo { get; set; }

        /// <summary>
        /// Gets or sets an id of a test assembly which this class belongs to.
        /// </summary>
        public int AssemblyInfoId { get; set; }

        /// <summary>
        /// Gets or sets test methods of a test class.
        /// </summary>
        public IEnumerable<TestMethodInfo> TestMethods { get; set; }
    }
}