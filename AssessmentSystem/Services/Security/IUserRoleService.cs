using System.Threading.Tasks;

namespace AssessmentSystem.Services.Security
{
    /// <summary>
    /// Represents a role service.
    /// </summary>
    public interface IUserRoleService
    {
        /// <summary>
        /// Finds a role using a role identifier.
        /// </summary>
        /// <param name="roleId">A role identifier.</param>
        /// <returns>An <see cref="IUserRole"/>.</returns>
        Task<IUserRole> FindRoleByIdAsync(string roleId);

        /// <summary>
        /// Finds a role using a role name.
        /// </summary>
        /// <param name="roleName">A role name.</param>
        /// <returns>An <see cref="IUserRole"/>.</returns>
        Task<IUserRole> FindRoleByNameAsync(string roleName);
    }
}
