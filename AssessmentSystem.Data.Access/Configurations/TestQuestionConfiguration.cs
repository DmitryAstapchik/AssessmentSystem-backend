using System.Data.Entity.ModelConfiguration;
using AssessmentSystem.Data.Access.ExerciseManagement;

namespace AssessmentSystem.Data.Access.Configurations
{
   internal sealed class TestQuestionConfiguration : EntityTypeConfiguration<TestQuestion>
    {
        public TestQuestionConfiguration()
        {
            ToTable("TestQuestions");
            HasKey<int>(q => q.Id);
            Property(q => q.Text).IsRequired();
            HasRequired<Test>(q => q.Test).WithMany(t => t.Questions).HasForeignKey<int>(q => q.TestId);
        }
    }
}
