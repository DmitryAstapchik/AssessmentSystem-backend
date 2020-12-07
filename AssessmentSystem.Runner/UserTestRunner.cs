using System;
using System.Collections.Generic;
using System.Reflection;
using AssessmentSystem.Factory;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Filters;

namespace AssessmentSystem.Runner
{
    public class UserTestRunner : MarshalByRefObject
    {
        public bool Run(byte[] testAssembly, string template, string testClassName, string testMethodName = null)
        {
            var asm = Assembly.Load(testAssembly);

            try
            {
                TestSourceFactory.Init(template);
            }
            catch (InvalidOperationException ex)
            {
                ErrorMessage = ex.Message;
                return false;
            }

            var runner = new NUnitTestAssemblyRunner(new DefaultTestAssemblyBuilder());
            runner.Load(asm, new Dictionary<string, object>());
            var listener = new CustomListener();

            ITestResult result;
            ITestFilter filter;
            string str = string.Empty;

            if (string.IsNullOrEmpty(testMethodName))
            {
                result = runner.Run(listener,
                    new ClassNameFilter(testClassName));
                filter = new ClassNameFilter(testClassName);
            }
            else
            {
                result = runner.Run(listener,
                    new AndFilter(new ClassNameFilter(testClassName),
                        new MethodNameFilter(testMethodName)));

                filter = new AndFilter(new ClassNameFilter(testClassName),
                        new MethodNameFilter(testMethodName));
            }

            ErrorMessage = listener.Message;

            return result.ResultState.Status != TestStatus.Failed;
        }

        public string ErrorMessage { get; private set; }


        public override object InitializeLifetimeService() => null;
    }

    public class CustomListener : ITestListener
    {
        public string Message { get; set; } = String.Empty;


        public void TestStarted(ITest test)
        {

        }

        public void TestFinished(ITestResult result)
        {
            if (result.ResultState.Status == TestStatus.Failed && string.IsNullOrEmpty(Message))
            {
                Message = result.Message;
            }
        }

        public void TestOutput(TestOutput output)
        {

        }
    }
}