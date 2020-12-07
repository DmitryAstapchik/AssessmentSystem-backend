using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AssessmentSystem.Models.ExerciseExecutor;

namespace AssessmentSystem.Services.ExerciseExecutor
{
    /// <summary>
    /// Represents a test assembly service contract.
    /// </summary>
    public interface ITestAssemblyService
    {
        /// <summary>
        /// Gets a test assembly by id.
        /// </summary>
        /// <param name="id">An id of a test assembly.</param>
        /// <returns>An instance of <see cref="TestAssemblyElements"/></returns>
        /// <exception cref="ArgumentException">Exception thrown when
        /// <paramref name="id"/>is less than 1.</exception>
        TestAssemblyElements GetTestAssembly(int id);

        /// <summary>
        /// Adds an assembly.
        /// </summary>
        /// <param name="rawAssembly">Assembly raw data.</param>
        /// <returns>Created assembly id.</returns>
        /// <exception cref="ArgumentException">Exception thrown when
        /// <paramref name="rawAssembly"/> is null.</exception>
        /// <exception cref="BadImageFormatException">Exception thrown when\
        /// <paramref name="rawAssembly"/> is not a valid assembly.</exception>
        Task<int> AddTestAssemblyAsync(byte[] rawAssembly);

        /// <summary>
        /// Removes a test assembly by id.
        /// </summary>
        /// <param name="id">An id of a test assembly.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentException">Exception thrown when
        /// <paramref name="id"/>is less than 1.</exception>
        Task RemoveTestAssemblyAsync(int id);

        /// <summary>
        /// Softly removes a test assembly by id.
        /// </summary>
        /// <param name="id">An id of a test assembly.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentException">Exception thrown when
        /// <paramref name="id"/>is less than 1.</exception>
        Task SoftRemoveTestAssemblyAsync(int id);

        /// <summary>
        /// Returns a list of assembly elements.
        /// </summary>
        /// <returns>Assembly elements list.</returns>
        IEnumerable<TestAssemblyElements> GetTestAssembliesElements();

        /// <summary>
        /// Returns a list of assemblies names and ids.
        /// </summary>
        /// <remarks>
        /// Item1 is the name of the assembly, item2 is the id of the assembly.
        /// </remarks>
        /// <returns>A list of assemblies names and ids.</returns>
        IEnumerable<Tuple<string, int>> GetAssembliesNamesAndIds();

        /// <summary>
        /// Check is task associated with assembly exist.
        /// </summary>
        /// <param name="assemblyId">An id of a test assembly.</param>
        /// <returns>Value indicating whether task is associated with assembly.</returns>
        bool IsTaskAssociatedWithAssemblyExist(int assemblyId);
    }
}
