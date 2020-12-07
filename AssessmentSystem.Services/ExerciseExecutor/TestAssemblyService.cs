using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AssessmentSystem.Data.Access.Context.Interfaces;
using AssessmentSystem.Models.ExerciseExecutor;
using AssessmentSystem.Services.Exceptions;
using AutoMapper;
using Fasterflect;
using NUnit.Framework;

namespace AssessmentSystem.Services.ExerciseExecutor
{
    /// <summary>
    /// Represents a test assembly service.
    /// </summary>
    internal class TestAssemblyService : ITestAssemblyService
    {
        private readonly ITestAssemblyContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestAssemblyService"/> class.
        /// </summary>
        /// <param name="context">An instance of <see cref="ITestAssemblyContext"/>.</param>
        /// <exception cref="ArgumentNullException">Exception thrown when
        /// <paramref name="context"/>is null.</exception>
        public TestAssemblyService(ITestAssemblyContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public TestAssemblyElements GetTestAssembly(int id)
        {
            if (id < 1)
            {
                throw new ArgumentException($"{nameof(id)} must be greater than 1.", nameof(id));
            }

            var assemblyInfo = _context.TestAssemblyInfo.FirstOrDefault(i => i.Id == id);

            if (assemblyInfo == null || assemblyInfo.IsSoftDeleted)
            {
                throw new AssemblyNotFoundException();
            }

            var classesElements = new List<TestClassElements>();
            var classesInfo = _context.TestClassInfo
                .Select(_ => new { _.AssemblyInfoId, _.Name, _.TestMethods }).ToArray();

            foreach (var classInfo in classesInfo)
            {
                if (classInfo.AssemblyInfoId == assemblyInfo.Id)
                {
                    var classElements = new TestClassElements
                    {
                        Name = classInfo.Name,
                        MethodsNames = classInfo.TestMethods.Select(_ => _.Name).ToArray()
                    };

                    classesElements.Add(classElements);
                }
            }

            var result = new TestAssemblyElements
            {
                AssemblyName = assemblyInfo.AssemblyName,
                TestClassesElements = classesElements
            };

            return result;
        }

        public async Task<int> AddTestAssemblyAsync(byte[] rawAssembly)
        {
            if (rawAssembly == null)
            {
                throw new ArgumentNullException(nameof(rawAssembly));
            }

            var assembly = Assembly.Load(rawAssembly);
            var moduleVersionId = assembly.ManifestModule.ModuleVersionId;
            if (!CheckMetaData(moduleVersionId))
            {
                int length = assembly.FullName.IndexOf(",", StringComparison.Ordinal);
                string assemblyName = assembly.FullName.Substring(0, length) + ".dll";

                var testAssemblyInfo = new AssessmentSystem.Data.Access.ExerciseExecutor.TestAssemblyInfo
                {
                    Data = rawAssembly,
                    AssemblyName = assemblyName,
                    IsSoftDeleted = false,
                    ModuleVersionId = moduleVersionId
                };

                _context.TestAssemblyInfo.Add(testAssemblyInfo);

                var testClasses = assembly.TypesWith(typeof(TestFixtureAttribute));
                foreach (var testClass in testClasses)
                {
                    var testClassInfo = new AssessmentSystem.Data.Access.ExerciseExecutor.TestClassInfo
                    {
                        Name = testClass.FullName,
                        AssemblyInfo = testAssemblyInfo
                    };

                    var methods = testClass.MethodsWith(Flags.InstancePublic, typeof(TestAttribute));
                    foreach (var method in methods)
                    {
                        var testMethodsInfo = new AssessmentSystem.Data.Access.ExerciseExecutor.TestMethodInfo
                        {
                            Name = method.Name,
                            ClassInfo = testClassInfo
                        };

                        testClassInfo.TestMethods.Add(testMethodsInfo);
                    }

                    _context.TestClassInfo.Add(testClassInfo);
                }

                await _context.SaveChangesAsync();
                return testAssemblyInfo.Id;
            }

            throw new AssemblyAlreadyExistsException();
        }

        public Task RemoveTestAssemblyAsync(int id)
        {
            if (id < 1)
            {
                throw new ArgumentException($"{nameof(id)} must be greater than 1.", nameof(id));
            }

            var temp = _context.TestAssemblyInfo.FirstOrDefault(cr => cr.Id == id);
            if (temp == null)
            {
                throw new TestAssemblyNotFoundException($"Assembly with {nameof(id)} = {id} not found.");
            }

            _context.TestAssemblyInfo.Remove(Mapper.Map<Data.Access.ExerciseExecutor.TestAssemblyInfo>(temp));
            return _context.SaveChangesAsync();
        }

        public Task SoftRemoveTestAssemblyAsync(int id)
        {
            if (id < 1)
            {
                throw new ArgumentException($"{nameof(id)} must be greater than 1.", nameof(id));
            }

            var temp = _context.TestAssemblyInfo.FirstOrDefault(cr => cr.Id == id);
            if (temp == null)
            {
                throw new TestAssemblyNotFoundException($"Assembly with {nameof(id)} = {id} not found.");
            }

            SoftDeleteAssembly(temp);
            return _context.SaveChangesAsync();
        }

        public IEnumerable<TestAssemblyElements> GetTestAssembliesElements()
        {
            var result = new List<TestAssemblyElements>();

            var assembliesInfo = _context.TestAssemblyInfo
                .Where(a => a.IsSoftDeleted == false)
                    .Select(_ => new { _.AssemblyName, _.Id }).ToArray();

            foreach (var assemblyInfo in assembliesInfo)
            {
                var classesElements = new List<TestClassElements>();
                var classesInfo = _context.TestClassInfo
                    .Select(_ => new { _.AssemblyInfoId, _.Name, _.TestMethods }).ToArray();

                foreach (var classInfo in classesInfo)
                {
                    if (classInfo.AssemblyInfoId == assemblyInfo.Id)
                    {
                        var classElements = new TestClassElements
                        {
                            Name = classInfo.Name,
                            MethodsNames = classInfo.TestMethods.Select(_ => _.Name).ToArray()
                        };

                        classesElements.Add(classElements);
                    }
                }

                var assemblyElements = new TestAssemblyElements
                {
                    AssemblyName = assemblyInfo.AssemblyName,
                    TestClassesElements = classesElements
                };

                result.Add(assemblyElements);
            }

            return result;
        }

        public IEnumerable<Tuple<string, int>> GetAssembliesNamesAndIds()
        {
            var data = _context.TestAssemblyInfo
                .Where(a => a.IsSoftDeleted == false)
                .Select(assembly => new { assembly.AssemblyName, assembly.Id })
                .AsEnumerable()
                .Select(assemblyData => Tuple.Create(assemblyData.AssemblyName, assemblyData.Id));

            return data.ToArray();
        }

        public bool IsTaskAssociatedWithAssemblyExist(int assemblyId)
        {
            var testClassInfo = _context.TestClassInfo.FirstOrDefault(
                result => result.AssemblyInfo.Id == assemblyId);
            return testClassInfo != null;
        }

        private void SoftDeleteAssembly(
            AssessmentSystem.Data.Access.ExerciseExecutor.TestAssemblyInfo assemblyInfo)
        {
            assemblyInfo.IsSoftDeleted = true;
        }

        private bool CheckMetaData(Guid moduleVersionId)
        {
            return _context.TestAssemblyInfo.Any(ta => ta.ModuleVersionId == moduleVersionId && !ta.IsSoftDeleted);
        }
    }
}
