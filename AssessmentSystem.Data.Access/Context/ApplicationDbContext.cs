using System.Data.Entity;
using System.Reflection;
using AssessmentSystem.Data.Access.ExerciseExecutor;
using AssessmentSystem.Data.Access.ExerciseManagement;
using AssessmentSystem.Data.Access.Migrations;
using AssessmentSystem.Data.Access.UserManagement;

namespace AssessmentSystem.Data.Access.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Configuration>());
        }

        public DbSet<Task> CandidateTasks { get; set; }

        public DbSet<TaskResult> CandidateTaskResults { get; set; }

        public DbSet<Test> CandidateTests { get; set; }

        public DbSet<TestQuestion> TestQuestions { get; set; }

        public DbSet<TestAnswerVariant> TestAnswerVariants { get; set; }

        public DbSet<TestResult> CandidateTestResults { get; set; }

        public DbSet<TestAssemblyInfo> TestAssemblies { get; set; }

        public DbSet<TestClassInfo> TestClasses { get; set; }

        public DbSet<TestMethodInfo> TestMethods { get; set; }

        public DbSet<Invite> Invites { get; set; }

        public DbSet<TaskTip> CandidateTaskTips { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.AddFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }
    }
}
