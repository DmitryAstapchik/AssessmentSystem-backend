using System;

namespace AssessmentSystem.Services.Exceptions
{
   public class InviteNotFoundException : Exception
    {
        public InviteNotFoundException()
        {
        }

        public InviteNotFoundException(string message)
            : base(message)
        {
        }

        public InviteNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
