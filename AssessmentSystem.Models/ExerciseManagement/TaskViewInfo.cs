using System.Collections.Generic;

namespace AssessmentSystem.Models.ExerciseManagement
{
    /// <summary>
    /// Represents a candidate task for view.
    /// </summary>
    public class TaskViewInfo : ExerciseForList
    {
        /// <summary>
        ///  Gets or sets a full description of the exercise.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a code template.
        /// </summary>
        public string CodeTemplate { get; set; }

        /// <summary>
        /// Gets or sets tips for a particular exercise.
        /// </summary>
        public IEnumerable<string> Tips { get; set; }
    }
}