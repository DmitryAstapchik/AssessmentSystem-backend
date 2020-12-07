using System;

namespace AssessmentSystem.Data.Access.ExerciseExecutor
{
    /// <summary>
    /// Represents an assembly for testing a candidate's task.
    /// </summary>
    public class TestAssemblyInfo
    {
        /// <summary>
        /// Gets or sets an id of a test assembly.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a name of a test assembly.
        /// </summary>
        public string AssemblyName { get; set; }

        /// <summary>
        /// Gets or sets a byte representation of a test assembly.
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether assembly was deleted.
        /// </summary>
        public bool IsSoftDeleted { get; set; }

        public Guid ModuleVersionId { get; set; }
    }
}