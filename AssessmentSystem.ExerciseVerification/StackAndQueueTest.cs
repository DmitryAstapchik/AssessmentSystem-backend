using System;
using AssessmentSystem.Core;
using AssessmentSystem.Factory;
using NUnit.Framework;

namespace AssessmentSystem.ExerciseVerification
{
    [TestFixture]
    public class StackAndQueueTest
    {
        [Test]
        [TestCaseSource(typeof(TestSourceFactory), nameof(TestSourceFactory.GetData))]
        public void TestQueueAndStack(IRunnable source)
        {
            var buff = source.Run();

            Assert.That(buff, Is.AssignableTo<IMessenger>());

            var testObj = buff as IMessenger;

            for (int i = 0; i < 5; i++)
            {
                testObj.SendMessage(i.ToString());
            }

            string actual = String.Empty;

            for (int i = 0; i < 5; i++)
            {
                actual += testObj.GetFirstMessage();
            }

            Assert.AreEqual("01234", actual, "GetFirstMessage() method has incorrect implementation");

            for (int i = 0; i < 5; i++)
            {
                testObj.SendMessage(i.ToString());
            }

            actual = String.Empty;

            for (int i = 0; i < 5; i++)
            {
                actual += testObj.GetLastMessage();
            }

            Assert.AreEqual("43210", actual, "GetLastMessage() method has incorrect implementation");
        }
    }
}
