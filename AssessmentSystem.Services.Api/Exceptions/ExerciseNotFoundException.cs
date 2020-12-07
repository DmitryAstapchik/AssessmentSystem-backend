using System;

namespace AssessmentSystem.Services.Exceptions
{
    public class ExerciseNotFoundException : Exception
    {
        public ExerciseNotFoundException()
        {
        }

        public ExerciseNotFoundException(string message)
            : base(message)
        {
        }

        public ExerciseNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
