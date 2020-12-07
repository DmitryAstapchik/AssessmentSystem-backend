using System.Data.Entity.ModelConfiguration;
using AssessmentSystem.Data.Access.ExerciseManagement;

namespace AssessmentSystem.Data.Access.Configurations
{
    /// <summary>
    /// Represents an invite table configuration for Entity Framework.
    /// </summary>
    internal sealed class TaskTipConfiguration : EntityTypeConfiguration<TaskTip>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskTipConfiguration"/> class.
        /// </summary>
        public TaskTipConfiguration()
        {
            ToTable("TaskTips");
            HasKey<int>(tip => tip.Id);
            Property(tip => tip.Text).IsRequired();
            HasRequired(tip => tip.CandidateTask).WithMany(task => task.Tips);
        }
    }
}
