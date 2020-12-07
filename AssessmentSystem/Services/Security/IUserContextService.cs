using System.Threading.Tasks;

namespace AssessmentSystem.Services.Security
{
    public interface IUserContextService
    {
        IUserContext GetCurrentUser();

        Task<IUserContext> GetCurrentUserAsync();
    }
}
