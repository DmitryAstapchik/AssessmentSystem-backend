using System.Collections.Generic;
using AssessmentSystem.Data.Access.ExerciseExecutor;

namespace AssessmentSystem.Data.Access.ExerciseManagement
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
        public virtual TestMethodInfo TestMethod { get; set; }

        /// <summary>
        /// Gets or sets a test method id.
        /// </summary>
        public int? TestMethodId { get; set; }

        /// <summary>
        /// Gets or sets a test class.
        /// </summary>
        public virtual TestClassInfo TestClass { get; set; }

        /// <summary>
        /// Gets or sets a test class id.
        /// </summary>
        public int TestClassId { get; set; }

        /// <summary>
        /// Gets or sets tips for a particular exercise.
        /// </summary>
        public virtual ICollection<TaskTip> Tips { get; set; } = new List<TaskTip>();
    }
}
