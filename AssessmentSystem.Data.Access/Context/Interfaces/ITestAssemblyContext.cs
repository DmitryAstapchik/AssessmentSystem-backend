using System.Threading.Tasks;
using AssessmentSystem.Data.Access.ExerciseExecutor;

namespace AssessmentSystem.Data.Access.Context.Interfaces
{
    /// <summary>
    /// Represents an assembly context.
    /// </summary>
    public interface ITestAssemblyContext
    {
        /// <summary>
        /// Gets a test assemblies info set.
        /// </summary>
        IEntitySet<TestAssemblyInfo> TestAssemblyInfo { get; }

        /// <summary>
        /// Gets an test classes info set.
        /// </summary>
        IEntitySet<TestClassInfo> TestClassInfo { get; }

        /// <summary>
        /// Gets an test methods info set.
        /// </summary>
        IEntitySet<TestMethodInfo> TestMethodInfos { get; }

        /// <summary>
        /// Saves all changes made in this context to an underlying storage.
        /// </summary>
        /// <returns>A task result of saving to a data base.</returns>
        Task SaveChangesAsync();
    }
}
