using System.Collections.Generic;
using AssessmentSystem.Models.Validators;
using FluentValidation.Attributes;

namespace AssessmentSystem.Models.ExerciseManagement
{
    /// <summary>
    /// Represents an exam task info.
    /// </summary>
    [Validator(typeof(TaskInfoValidator))]
    public class TaskInfo : Exercise
    {
        /// <summary>
        /// Gets or sets a code template.
        /// </summary>
        public string CodeTemplate { get; set; }

        /// <summary>
        /// Gets or sets an assembly name.
        /// </summary>
        public string AssemblyName { get; set; }

        /// <summary>
        /// Gets or sets a test class name.
        /// </summary>
        public string TestClassName { get; set; }

        /// <summary>
        /// Gets or sets a test method name.
        /// </summary>
        /// <remarks>
        /// If the method is not specified then
        /// the value of the field is an empty string.
        /// </remarks>
        public string TestMethodName { get; set; }

        /// <summary>
        /// Gets or sets tips for a particular exercise.
        /// </summary>
        public IEnumerable<string> Tips { get; set; }
    }
}
