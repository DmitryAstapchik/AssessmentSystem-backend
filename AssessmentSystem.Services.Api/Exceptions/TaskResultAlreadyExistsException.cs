using System;

namespace AssessmentSystem.Services.Exceptions
{
    /// <summary>
    /// When we try to create new task result, but task result with such id already exists.
    /// </summary>
    public class TaskResultAlreadyExistsException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskResultAlreadyExistsException"/> class.
        /// </summary>
        public TaskResultAlreadyExistsException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskResultAlreadyExistsException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public TaskResultAlreadyExistsException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskResultAlreadyExistsException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Inner excetpion.</param>
        public TaskResultAlreadyExistsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
