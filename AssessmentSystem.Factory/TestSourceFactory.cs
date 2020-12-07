using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AssessmentSystem.Core;
using Fasterflect;
using Microsoft.CSharp;
using NUnit.Framework;

namespace AssessmentSystem.Factory
{
    public static class TestSourceFactory
    {
        private static IRunnable _obj;

        public static IEnumerable<TestCaseData> GetData()
        {
            yield return new TestCaseData(_obj);
        }

        public static void Init(string code)
        {
            var provider = new CSharpCodeProvider();
            var parameters = new CompilerParameters(new[] { "System.dll", "System.Data.dll", "mscorlib.dll", typeof(IRunnable).Assembly.Location })
            { GenerateInMemory = true, GenerateExecutable = false };

            var result = provider.CompileAssemblyFromSource(parameters, code);

            if (result.Errors.HasErrors)
            {
                string message = result.Errors.OfType<CompilerError>().Aggregate(string.Empty, (current, error) => current + (error.ErrorText + Environment.NewLine));
                throw new InvalidOperationException(message);
            }

            var item = result.CompiledAssembly.TypesImplementing<IRunnable>().FirstOrDefault();
            _obj = (IRunnable)Activator.CreateInstance(item);
        }
    }
}
