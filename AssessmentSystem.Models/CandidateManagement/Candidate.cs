using System;

namespace AssessmentSystem.Models.CandidateManagement
{
    /// <summary>
    /// Represents a candidate info.
    /// </summary>
    public class Candidate
    {
        /// <summary>
        /// Gets or sets a user profile identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets a user firstname.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets a user lastname.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets score that the candidate received for completing the exercises.
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// Gets or sets maximum score that the candidate can receive for completing the exercises.
        /// </summary>
        public int MaximumScore { get; set; }
    }
}
