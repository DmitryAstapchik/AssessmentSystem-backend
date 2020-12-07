using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssessmentSystem.Data.Access.ExerciseExecutor
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
        public virtual TestAssemblyInfo AssemblyInfo { get; set; }

        /// <summary>
        /// Gets or sets an id of a test assembly which this class belongs to.
        /// </summary>
        [ForeignKey(nameof(AssemblyInfo))]
        public int AssemblyInfoId { get; set; }

        /// <summary>
        /// Gets or sets test methods of a test class.
        /// </summary>
        public virtual ICollection<TestMethodInfo> TestMethods { get; set; } = new List<TestMethodInfo>();
    }
}