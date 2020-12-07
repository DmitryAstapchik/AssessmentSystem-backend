using System.Data.Entity;
using AssessmentSystem.Identity.Migrations;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AssessmentSystem.Identity
{
    /// <summary>
    /// Represents a <see cref="DbContext"/> for ASP.NET Identity infrastructure.
    /// </summary>
    public class ApplicationIdentityDbContext : IdentityDbContext<ApplicationUser>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationIdentityDbContext"/> class.
        /// </summary>
        public ApplicationIdentityDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationIdentityDbContext, Configuration>());
        }

        /// <summary>
        /// Gets or sets a <see cref="DbSet{TEntity}"/> for user profile table.
        /// </summary>
        public virtual DbSet<ApplicationUserProfile> UserProfiles { get; set; }
    }
}
