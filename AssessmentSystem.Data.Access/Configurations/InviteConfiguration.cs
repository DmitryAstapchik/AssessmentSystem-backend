using System.Data.Entity.ModelConfiguration;
using AssessmentSystem.Data.Access.UserManagement;

namespace AssessmentSystem.Data.Access.Configurations
{
    /// <summary>
    /// Represents an invite table configuration for Entity Framework.
    /// </summary>
    internal sealed class InviteConfiguration : EntityTypeConfiguration<Invite>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InviteConfiguration"/> class.
        /// </summary>
        public InviteConfiguration()
        {
            ToTable("Invites");
            HasKey<int>(i => i.Id);
            Property(i => i.Token).HasColumnName("Token").IsRequired();
            Property(i => i.Email).HasColumnName("Email").IsRequired();
            Property(i => i.RoleName).HasColumnName("RoleName").IsRequired();
            Property(i => i.ExpiredDate).HasColumnName("ExpirationDate").IsRequired();
            Property(i => i.UserId).HasColumnName("UserId").IsOptional();
        }
    }
}
