using System;

namespace AssessmentSystem.Services.Exceptions
{
    public class TaskResultNotFoundException : Exception
    {
        public TaskResultNotFoundException()
        {
        }

        public TaskResultNotFoundException(string message)
            : base(message)
        {
        }

        public TaskResultNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
