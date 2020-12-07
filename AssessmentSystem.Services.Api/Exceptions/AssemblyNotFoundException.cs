using System;

namespace AssessmentSystem.Services.Exceptions
{
    public class AssemblyNotFoundException : Exception
    {
        public AssemblyNotFoundException()
        {
        }

        public AssemblyNotFoundException(string message)
            : base(message)
        {
        }

        public AssemblyNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
