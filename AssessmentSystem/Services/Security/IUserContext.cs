using System;

namespace AssessmentSystem.Services.Security
{
    public interface IUserContext
    {
        [Obsolete("Don't use this property because it will be removed.")]
        string IdentityId { get; }

        Guid Id { get; }
    }
}