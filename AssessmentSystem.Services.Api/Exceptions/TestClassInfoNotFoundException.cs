using System;

namespace AssessmentSystem.Services.Exceptions
{
    public class TestClassInfoNotFoundException : Exception
    {
        public TestClassInfoNotFoundException()
        {
        }

        public TestClassInfoNotFoundException(string message)
            : base(message)
        {
        }

        public TestClassInfoNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
