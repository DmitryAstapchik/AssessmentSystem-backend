using System;
using System.Threading.Tasks;
using AssessmentSystem.Data.Access.Context.Interfaces;
using AssessmentSystem.Data.Access.UserManagement;

namespace AssessmentSystem.Data.Access.Context
{
    /// <summary>
    /// Represents a context of invites.
    /// </summary>
    public class InvitesContext : IInvitesContext
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvitesContext"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ApplicationDbContext"/></param>
        public InvitesContext(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Gets an invites set.
        /// </summary>
        public IEntitySet<Invite> Invites => new EntitySet<Invite>(_context.Invites);

        /// <summary>
        /// Saves changes to database asynchronously.
        /// </summary>
        /// <returns>A task of saving all changes.</returns>
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
