using AssessmentSystem.Core;

namespace AssessmentSystem.Examples
{
    public class TestClass : IRunnable
    {
        public object Run()
        {
            return new Calculator();
        }
    }
}