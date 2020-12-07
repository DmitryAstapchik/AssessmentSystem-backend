using System.Data.Entity.ModelConfiguration;
using AssessmentSystem.Data.Access.ExerciseManagement;

namespace AssessmentSystem.Data.Access.Configurations
{
    /// <summary>
    /// Represents an invite table configuration for Entity Framework.
    /// </summary>
    internal sealed class ExerciseConfiguration : EntityTypeConfiguration<Exercise>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExerciseConfiguration"/> class.
        /// </summary>
        public ExerciseConfiguration()
        {
            ToTable("Exercises");
            HasKey<int>(i => i.Id);
            Property(i => i.Name).IsRequired();
            Property(i => i.Subject).IsRequired();
            Property(i => i.Description).IsRequired();
            Property(i => i.MaximumScore).IsRequired();
        }
    }
}