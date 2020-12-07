using AssessmentSystem.Identity;

namespace AssessmentSystem.Services.Security
{
    /// <summary>
    /// Represents a user role.
    /// </summary>
    public class UserRole : IUserRole
    {
        /// <summary>
        /// Gets or sets a user role identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets a user role name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Creates a new instance of the <see cref="UserRole"/> class using <see cref="ApplicationRole"/> as a parameter.
        /// </summary>
        /// <param name="applicationRole">A <see cref="ApplicationRole"/>.</param>
        /// <returns>A <see cref="UserRole"/>.</returns>
        public static UserRole Create(ApplicationRole applicationRole)
        {
            return new UserRole
            {
                Id = applicationRole.Id,
                Name = applicationRole.Name
            };
        }
    }
}