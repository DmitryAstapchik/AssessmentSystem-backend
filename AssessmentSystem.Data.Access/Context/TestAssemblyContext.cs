using System;
using System.Threading.Tasks;
using AssessmentSystem.Data.Access.Context.Interfaces;
using AssessmentSystem.Data.Access.ExerciseExecutor;

namespace AssessmentSystem.Data.Access.Context
{
    internal class TestAssemblyContext : ITestAssemblyContext
    {
        private readonly ApplicationDbContext _context;

        public TestAssemblyContext(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            TestAssemblyInfo = new EntitySet<TestAssemblyInfo>(_context.TestAssemblies);
            TestClassInfo = new EntitySet<TestClassInfo>(_context.TestClasses);
            TestMethodInfos = new EntitySet<TestMethodInfo>(_context.TestMethods);
        }

        public IEntitySet<TestAssemblyInfo> TestAssemblyInfo { get; }

        public IEntitySet<TestClassInfo> TestClassInfo { get; }

        public IEntitySet<TestMethodInfo> TestMethodInfos { get; }

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
