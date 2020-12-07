using System;
using AssessmentSystem.Data.Access.Context.Interfaces;
using AssessmentSystem.Data.Access.ExerciseExecutor;
using AssessmentSystem.Data.Access.ExerciseManagement;

namespace AssessmentSystem.Data.Access.Context
{
    internal class ExercisesContext : IExercisesContext
    {
        private readonly ApplicationDbContext _context;

        public ExercisesContext(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            CandidateTasks = new EntitySet<ExerciseManagement.Task>(_context.CandidateTasks);
            CandidateTests = new EntitySet<Test>(_context.CandidateTests);
            TestQuestions = new EntitySet<TestQuestion>(_context.TestQuestions);
            TestAnswerVariants = new EntitySet<TestAnswerVariant>(_context.TestAnswerVariants);
            CandidateTaskResults = new EntitySet<TaskResult>(_context.CandidateTaskResults);
            CandidateTestResults = new EntitySet<TestResult>(_context.CandidateTestResults);
            CandidateTaskTips = new EntitySet<TaskTip>(_context.CandidateTaskTips);
        }

        public IEntitySet<ExerciseManagement.Task> CandidateTasks { get; }

        public IEntitySet<Test> CandidateTests { get; }

        public IEntitySet<TestQuestion> TestQuestions { get; }

        public IEntitySet<TestAnswerVariant> TestAnswerVariants { get; }

        public IEntitySet<TaskResult> CandidateTaskResults { get; }

        public IEntitySet<TestResult> CandidateTestResults { get; }

        public IEntitySet<TaskTip> CandidateTaskTips { get; }

        public async System.Threading.Tasks.Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
