using Microsoft.AspNet.Identity.EntityFramework;

namespace AssessmentSystem.Identity
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole()
        {
        }

        public ApplicationRole(string name)
            : base(name)
        {
        }
    }
}
