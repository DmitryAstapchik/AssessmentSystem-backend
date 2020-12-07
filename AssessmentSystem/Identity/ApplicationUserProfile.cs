using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssessmentSystem.Identity
{
    [Table("AspNetUserProfiles")]
    public class ApplicationUserProfile
    {
        [Key]
        [ForeignKey("User")]
        public string UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
