using NUnit.Framework.Internal;

namespace AssessmentSystem.Runner
{
    /// <summary>
    /// Represents a test filter contract.
    /// </summary>
    public interface INUnitFilterFactory
    {
        /// <summary>
        /// Generates a test filter from class name.
        /// </summary>
        /// <returns>A test filter.</returns>
        TestFilter TestFilterFromClass();

        /// <summary>
        /// Generates a test filter from class and method names. 
        /// </summary>
        /// <returns>A test filter.</returns>
        TestFilter TestFilterFromClassAndMethod();
    }
}
