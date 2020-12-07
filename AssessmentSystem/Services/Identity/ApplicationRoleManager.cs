using System;
using System.Threading.Tasks;
using AssessmentSystem.Identity;
using AssessmentSystem.Services.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using ISecurityRole = AssessmentSystem.Services.Security.IUserRole;

namespace AssessmentSystem.Services.Identity
{
    /// <summary>
    /// Represents an application role manager.
    /// </summary>
    public class ApplicationRoleManager : RoleManager<ApplicationRole>, IApplicationRoleManager, IUserRoleService
    {
        public ApplicationRoleManager(IRoleStore<ApplicationRole, string> store)
            : base(store)
        {
        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            var manager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(context.Get<ApplicationIdentityDbContext>()));

            manager.RoleValidator = new RoleValidator<ApplicationRole>(manager);
            return manager;
        }

        /// <summary>
        /// Finds a role using a role identifier.
        /// </summary>
        /// <param name="roleId">A role identifier.</param>
        /// <returns>An <see cref="IUserRole"/>.</returns>
        public async Task<ISecurityRole> FindRoleByIdAsync(string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                throw new ArgumentNullException(nameof(roleId));
            }

            var applicationRole = await FindByIdAsync(roleId);
            return UserRole.Create(applicationRole);
        }

        /// <summary>
        /// Finds a role using a role name.
        /// </summary>
        /// <param name="roleName">A role name.</param>
        /// <returns>An <see cref="IUserRole"/>.</returns>
        /// <exception cref="RoleNotFoundException">Role not found.</exception>
        public async Task<ISecurityRole> FindRoleByNameAsync(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentNullException(nameof(roleName));
            }

            var applicationRole = await FindByNameAsync(roleName);
            if (applicationRole == null)
            {
                throw new RoleNotFoundException($"Application role '{roleName}' is not found.");
            }

            return UserRole.Create(applicationRole);
        }
    }
}
