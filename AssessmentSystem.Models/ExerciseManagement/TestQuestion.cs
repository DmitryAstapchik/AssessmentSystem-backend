using System;
using System.Collections.Generic;
using System.Linq;

namespace AssessmentSystem.Models.ExerciseManagement
{
    /// <summary>
    /// Represents a question in the test.
    /// </summary>
    public class TestQuestion
    {
        private static Random _randomizer = new Random();

        private IEnumerable<TestAnswerVariant> _answerVariants;

        /// <summary>
        /// Gets or sets id of the question.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets full text of the question.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets all variants of answer to this question.
        /// </summary>
        public IEnumerable<TestAnswerVariant> AnswerVariants
        {
            get
            {
                return _answerVariants == null ? null : _answerVariants.OrderBy(v => _randomizer.Next());
            }

            set
            {
                _answerVariants = value;
            }
        }
    }
}
