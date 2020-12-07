using System;

namespace AssessmentSystem.Services.Security
{
    public class RoleNotFoundException : Exception
    {
        public RoleNotFoundException()
        {
        }

        public RoleNotFoundException(string message)
            : base(message)
        {
        }

        public RoleNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}