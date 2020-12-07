using System.Threading.Tasks;
using AssessmentSystem.Data.Access.UserManagement;

namespace AssessmentSystem.Data.Access.Context.Interfaces
{
    /// <summary>
    /// Represents a context of invites.
    /// </summary>
    public interface IInvitesContext
    {
        /// <summary>
        /// Gets an invites set.
        /// </summary>
        IEntitySet<Invite> Invites { get; }

        /// <summary>
        /// Saves changes to database asynchronously.
        /// </summary>
        /// <returns>A task of saving all changes.</returns>
        Task SaveChangesAsync();
    }
}
