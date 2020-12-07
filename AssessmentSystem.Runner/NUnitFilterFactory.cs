using NUnit.Framework.Internal;
using System;

namespace AssessmentSystem.Runner
{
    /// <summary>
    /// Represents a filter factory for NUnit 3.9.0
    /// </summary>
    [Serializable]
    internal class NUnitFilterFactory: INUnitFilterFactory
    {
        private string testFilterFromClass = "<filter><class>{testClassName}</class></filter>";
        private string testFilterFromClassAndMethod = "<filter><and><class>{testClassName}</class><method>{testMethodName}</method></and></filter>";

        /// <summary>
        /// Generates a test filter from class name.
        /// </summary>
        /// <returns>A test filter.</returns>
        public TestFilter TestFilterFromClass()
        {
            return TestFilter.FromXml(testFilterFromClass);
        }

        /// <summary>
        /// Generates a test filter from class and method names. 
        /// </summary>
        /// <returns>A test filter.</returns>
        public TestFilter TestFilterFromClassAndMethod()
        {
            return TestFilter.FromXml(testFilterFromClassAndMethod);
        }

    }
}
