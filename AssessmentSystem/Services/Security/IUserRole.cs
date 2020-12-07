namespace AssessmentSystem.Services.Security
{
    /// <summary>
    /// Represents a role.
    /// </summary>
    public interface IUserRole
    {
        /// <summary>
        /// Gets a role identifier.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets a role name.
        /// </summary>
        string Name { get; }
    }
}
