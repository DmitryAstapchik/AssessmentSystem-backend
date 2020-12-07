using System;

namespace AssessmentSystem.Services.Exceptions
{
    /// <summary>
    /// When we try to validate task, but the timer went out.
    /// </summary>
    public class TimerOutException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimerOutException"/> class.
        /// </summary>
        public TimerOutException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimerOutException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public TimerOutException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimerOutException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Inner excetpion.</param>
        public TimerOutException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
