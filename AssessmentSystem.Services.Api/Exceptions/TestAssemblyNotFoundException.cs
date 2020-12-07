using System;

namespace AssessmentSystem.Services.Exceptions
{
    public class TestAssemblyNotFoundException : Exception
    {
        public TestAssemblyNotFoundException()
        {
        }

        public TestAssemblyNotFoundException(string message)
            : base(message)
        {
        }

        public TestAssemblyNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
