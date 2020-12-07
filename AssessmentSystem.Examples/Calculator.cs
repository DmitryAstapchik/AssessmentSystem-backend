using AssessmentSystem.Core;

namespace AssessmentSystem.Examples
{
    public class Calculator : ICalculator
    {
        public int Sum(int x, int y)
        {
            return x + y;
        }

        public int Divide(int x, int y)
        {
            return x / y;
        }

        public int Subst(int x, int y)
        {
            return x - y;
        }
    }

    public class CalcActivator : IRunnable
    {
        public object Run()
        {
            return new Calculator();
        }
    }
}