using System;
using System.Collections.Generic;
using System.Linq;

namespace AssessmentSystem.Models.ExerciseManagement
{
    /// <summary>
    /// Represents an exam test.
    /// </summary>
    public class Test : Exercise
    {
        private static Random _randomizer = new Random();

        private IEnumerable<TestQuestion> _questions;

        /// <summary>
        /// Gets or sets a number of questions for a test.
        /// </summary>
        public IEnumerable<TestQuestion> Questions
        {
            get
            {
                return _questions == null ? null : _questions.OrderBy(q => _randomizer.Next());
            }

            set
            {
                _questions = value;
            }
        }
    }
}