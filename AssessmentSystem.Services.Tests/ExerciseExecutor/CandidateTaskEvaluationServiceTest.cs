/*using System;
using System.Collections.Generic;
using System.IO;
using AssessmentSystem.Models.ExerciseExecutor;
using AssessmentSystem.Models.ExerciseManagement;
using AssessmentSystem.Runner;
using NUnit.Framework;

namespace AssessmentSystem.Services.ExerciseExecutor
{
    /// <summary>
    /// Represents a test class that checks CandidateTaskEvaluationService.
    /// </summary>
    [TestFixture]
    internal class CandidateTaskEvaluationServiceTest
    {
        private string _candidatesCode = @"
            using System;
            using System.Collections;
            using System.Collections.Generic;
            using AssessmentSystem.Core;

            namespace AssessmentSystem.Examples
            {
                public class Messenger : IMessenger
                {
                    private Stack<string> _stack = new Stack<string>();
                    private Queue<string> _queue = new Queue<string>();

                    public void SendMessage(string message)
                    {
                        _queue.Enqueue(message);
                        _stack.Push(message);
                    }

                    public string GetLastMessage()
                    {
                        return _stack.Pop();
                    }

                    public string GetFirstMessage()
                    {
                        return _queue.Dequeue();
                    }
                }

                public class TaskActivator : IRunnable
                {
                    public object Run()
                    {
                        return new Messenger();
                    }
                }
            }";

        private string _testClassName = "StackAndQueueTest";
        private string _testMethodName = "TestQueueAndStack";

        /// <summary>
        /// A test checks a method that runs a task.
        /// Method Run() takes 4 parameters: byte[] testAssembly, string template, string testClassName, string testMethodName.
        /// TestAssembly ia an assembly that contains test for checking candidate's code.
        /// Template is a code template of a task that should be done by a candidate.
        /// TestClassName ia a name of a class in a test assembly that contains methods for checkig a task that was made by a code template.
        /// TestMethodName is a name of a method in a test that was made to check a candidate's code.
        /// </summary>
        [Test]
        public void RunTask()
        {
            byte[] testAssembly = GetTestAssemblyBytes();
            var runner = new UserTestRunner();
            bool result = runner.Run(testAssembly, _candidatesCode, _testClassName, _testMethodName);

            Assert.AreEqual(true, result);
        }

        /// <summary>
        /// Checks a CandidateTaskRunner.
        /// </summary>
        [Test]
        public void CheckCandidateTaskRunner()
        {
            var runner = new CandidateTaskRunner();
            TaskRunResult result = runner.Run(CreateTask(), _candidatesCode);

            Assert.AreEqual(true, result.Success);
        }

        /// <summary>
        /// Gets an assembly that contains tests for checking candidate's code in byte representation.
        /// </summary>
        /// <returns>A byte[] array of an assembly that contains tests for checking candidate's code.</returns>
        private byte[] GetTestAssemblyBytes()
        {
            string assemblyPath = AppDomain.CurrentDomain.BaseDirectory + "\\..\\..\\";
            byte[] testAssembly = File.ReadAllBytes(Path.Combine(assemblyPath, "TestAssemblies\\AssessmentSystem.ExerciseVerification.dll"));
            return testAssembly;
        }

        /// <summary>
        /// Creates a candidate's task.
        /// </summary>
        /// <returns>candidate's task</returns>
        private CandidateTask CreateTask()
        {
            byte[] testAssembly = GetTestAssemblyBytes();

            TestAssemblyInfo testAssemblyInfo = new TestAssemblyInfo
            {
                Id = 1,
                Data = testAssembly,
                AssemblyName = "AssessmentSystem.Tests"
            };

            TestClassInfo testClassInfo = new TestClassInfo
            {
                Id = 1,
                Name = "AssessmentSystem.Tests.StackAndQueueTest",
                AssemblyInfoId = 1,
                AssemblyInfo = testAssemblyInfo,
                TestMethods = new List<TestMethodInfo>() { }
            };

            TestMethodInfo testMethodInfo = new TestMethodInfo
            {
                Id = 1,
                Name = "TestQueueAndStack",
                ClassInfoId = 2,
                ClassInfo = testClassInfo
            };

            CandidateTask candidateTask =
                new CandidateTask
                {
                    Id = 1,
                    Name = "task1",
                    TestClass = testClassInfo,
                    Subject = "DataStructures",
                    MaximumScore = 100,
                    Description = "make a task",
                    TestClassId = 1,
                    TestMethod = testMethodInfo,
                    TestMethodId = 1,
                    Tips = new List<string>() { "tip1", "tip2", "tip3" },
                    CodeTemplate = @"using System;
                    using System.Collections;
                    using System.Collections.Generic;
                    using AssessmentSystem.Core;

                    namespace AssessmentSystem.Examples
                    {
                        public class Messenger : IMessenger
                        {
                            //отправляем сообщение
                            public void SendMessage(string message)
                            {
                                throw new NotImplementedException();
                            }

                            //получаем сообщение в порядке обратном поступлению (из конца в начало)
                            public string GetLastMessage()
                            {
                                throw new NotImplementedException();
                            }

                            //получаем сообщение в порядке поступления (с начала в конец)
                            public string GetFirstMessage()
                            {
                                throw new NotImplementedException();
                            }
                        }

                        public class TaskActivator : IRunnable
                        {
                            public object Run()
                            {
                                return new Messenger();
                            }
                        }
                    }"
                };

            return candidateTask;
        }
    }
}
*/