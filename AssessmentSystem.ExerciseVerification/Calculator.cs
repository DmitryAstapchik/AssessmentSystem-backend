using AssessmentSystem.Core;
using AssessmentSystem.Factory;
using NUnit.Framework;

namespace AssessmentSystem.ExerciseVerification
{
    [TestFixture]
    public class Calculator
    {

        [Test]
        [Order(1)]
        [TestCaseSource(typeof(TestSourceFactory), nameof(TestSourceFactory.GetData))]
        public void ExampleTestSum(IRunnable data)
        {
            var result = data.Run();

            Assert.That(result, Is.AssignableTo<ICalculator>());
            ICalculator obj = (ICalculator)result;

            Assert.AreEqual(3, obj.Sum(1, 2), "Метод суммирования реализован неверно");
            Assert.AreEqual(5, obj.Sum(2, 3), "Метод суммирования реализован неверно");
            Assert.AreEqual(0, obj.Sum(0, 0), "Метод суммирования реализован неверно");
        }

        [Test]
        [Order(2)]
        [TestCaseSource(typeof(TestSourceFactory), nameof(TestSourceFactory.GetData))]
        public void ExampleTestDivide(IRunnable data)
        {
            var result = data.Run();

            Assert.That(result, Is.AssignableTo<ICalculator>());
            ICalculator obj = (ICalculator)result;

            Assert.AreEqual(3, obj.Divide(6, 2), "Метод деления реализован неверно");
            Assert.AreEqual(2, obj.Divide(4, 2), "Метод деления реализован неверно");
            Assert.AreEqual(8, obj.Divide(16, 2), "Метод деления реализован неверно");
        }

        [Test]
        [Order(3)]
        [TestCaseSource(typeof(TestSourceFactory), nameof(TestSourceFactory.GetData))]
        public void ExampleTestSubstract(IRunnable data)
        {
            var result = data.Run();

            Assert.That(result, Is.AssignableTo<ICalculator>());
            ICalculator obj = (ICalculator)result;

            Assert.AreEqual(3, obj.Subst(8, 5), "Метод вычитания реализован неверно");
            Assert.AreEqual(13, obj.Subst(18, 5), "Метод вычитания реализован неверно");
            Assert.AreEqual(4, obj.Subst(9, 5), "Метод вычитания реализован неверно");
        }
    }
}
