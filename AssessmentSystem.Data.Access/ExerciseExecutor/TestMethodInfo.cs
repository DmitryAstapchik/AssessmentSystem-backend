using System.ComponentModel.DataAnnotations.Schema;

namespace AssessmentSystem.Data.Access.ExerciseExecutor
{
    /// <summary>
    /// Represents a method for testing a candidate's task.
    /// </summary>
    public class TestMethodInfo
    {
        /// <summary>
        /// Gets or sets an id of a test method.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a name of a test method.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a test class which this method belongs to.
        /// </summary>
        public virtual TestClassInfo ClassInfo { get; set; }

        /// <summary>
        /// Gets or sets an id of a test class which this method belongs to.
        /// </summary>
        [ForeignKey(nameof(ClassInfo))]
        public int ClassInfoId { get; set; }
    }
}