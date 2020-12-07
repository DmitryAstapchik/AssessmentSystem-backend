using System.Data.Entity.ModelConfiguration;
using AssessmentSystem.Data.Access.ExerciseManagement;

namespace AssessmentSystem.Data.Access.Configurations
{
    internal sealed class TestAnswerVariantConfiguration : EntityTypeConfiguration<TestAnswerVariant>
    {
        public TestAnswerVariantConfiguration()
        {
            ToTable("TestAnswerVariants");
            HasKey<int>(v => v.Id);
            Property(v => v.Text).IsRequired();
            Property(v => v.IsCorrect).IsRequired();
            HasRequired<TestQuestion>(v => v.Question).WithMany(q => q.AnswerVariants).HasForeignKey<int>(v => v.QuestionId);
        }
    }
}
