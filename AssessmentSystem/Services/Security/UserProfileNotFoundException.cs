using System;
using System.Runtime.Serialization;

namespace AssessmentSystem.Services.Security
{
    [Serializable]
    public class UserProfileNotFoundException : Exception
    {
        public UserProfileNotFoundException()
        {
        }

        public UserProfileNotFoundException(string message)
            : base(message)
        {
        }

        public UserProfileNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected UserProfileNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
