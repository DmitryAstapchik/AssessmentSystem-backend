using System.Data.Entity.ModelConfiguration;
using AssessmentSystem.Data.Access.ExerciseManagement;

namespace AssessmentSystem.Data.Access.Configurations
{
    internal sealed class TestConfiguration : EntityTypeConfiguration<Test>
    {
        public TestConfiguration()
        {
            ToTable("Exercises");
            HasMany<TestQuestion>(t => t.Questions).WithRequired(q => q.Test).HasForeignKey<int>(q => q.TestId);
        }
    }
}
