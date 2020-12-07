using System;
using System.Collections.Generic;
using System.Linq;
using AssessmentSystem.Data.Access.Context.Interfaces;
using AssessmentSystem.Data.Access.ExerciseExecutor;
using Moq;
using NUnit.Framework;

namespace AssessmentSystem.Services.ExerciseManagement
{
    [TestFixture]
    internal class ExerciseServiceTests
    {
        private ExerciseService _service;
        private Mock<IExercisesContext> _exerciseContextMock;
        private Mock<IExercisePoolService> _exercisePoolMock;
        private Mock<ITestAssemblyContext> _testAssemblyContextMock;

        [SetUp]
        public void Init()
        {
            _exercisePoolMock = new Mock<IExercisePoolService>();
            _exerciseContextMock = new Mock<IExercisesContext>();
            _testAssemblyContextMock = new Mock<ITestAssemblyContext>();

            _service = new ExerciseService(
                _exerciseContextMock.Object,
                _exercisePoolMock.Object,
                _testAssemblyContextMock.Object);
        }

        [Test]
        public void Ctor_NullArgs_ThrowsException()
        {
            Assert.That(() => new ExerciseService(null, _exercisePoolMock.Object, _testAssemblyContextMock.Object), Throws.TypeOf<ArgumentNullException>());
            Assert.That(() => new ExerciseService(_exerciseContextMock.Object, null, _testAssemblyContextMock.Object), Throws.TypeOf<ArgumentNullException>());
            Assert.That(() => new ExerciseService(_exerciseContextMock.Object, _exercisePoolMock.Object, null), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void GetItems_GuidEmptyArg_ThrowsException()
        {
            Assert.That(
                () => _service.GetCandidateExerciseList(Guid.Empty), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void GetItems_ZeroSet_ReturnsEmptyCollection()
        {
            // Arrange.
            var exercises = new AssessmentSystem.Models.ExerciseManagement.Exercise[] { };
            _exercisePoolMock.Setup(pool => pool.GetActiveExerciseSet()).Returns(exercises);

            var fakeTaskSet = new FakeEntitySet<TaskResult>(new List<TaskResult>());
            var fakeTestSet = new FakeEntitySet<TestResult>(new List<TestResult>());

            _exerciseContextMock.SetupGet(t => t.CandidateTaskResults).Returns(fakeTaskSet);
            _exerciseContextMock.SetupGet(t => t.CandidateTestResults).Returns(fakeTestSet);

            _service = new ExerciseService(
                _exerciseContextMock.Object,
                _exercisePoolMock.Object,
                _testAssemblyContextMock.Object);

            // Act.
            var result = _service.GetCandidateExerciseList(applicationUserId: Guid.NewGuid());

            // Assert.
            Assert.AreEqual(0, result.Count());
        }

        [Test]
        public void GetItems_TwoItems_ReturnTwoItems()
        {
            // Arrange.
            var candidateTask = new AssessmentSystem.Models.ExerciseManagement.Task { Id = 1 };
            var candidateTest = new AssessmentSystem.Models.ExerciseManagement.Test { Id = 2 };
            var exercises = new AssessmentSystem.Models.ExerciseManagement.Exercise[] { candidateTask, candidateTest };

            _exercisePoolMock.Setup(pool => pool.GetActiveExerciseSet()).Returns(exercises);

            var candidateTaskResult = new TaskResult
            {
                CreatorId = Guid.Empty,
                CandidateExercise = new AssessmentSystem.Data.Access.ExerciseManagement.Task { Id = 1 }
            };

            var candidateTestResult = new TestResult
            {
                CreatorId = Guid.Empty,
                CandidateExercise = new AssessmentSystem.Data.Access.ExerciseManagement.Exercise { Id = 2 }
            };

            var fakeTaskSet = new FakeEntitySet<TaskResult>(new List<TaskResult> { candidateTaskResult });
            var fakeTestSet = new FakeEntitySet<TestResult>(new List<TestResult> { candidateTestResult });

            _exerciseContextMock.SetupGet(t => t.CandidateTaskResults).Returns(fakeTaskSet);
            _exerciseContextMock.SetupGet(t => t.CandidateTestResults).Returns(fakeTestSet);

            _service = new ExerciseService(
                _exerciseContextMock.Object,
                _exercisePoolMock.Object,
                _testAssemblyContextMock.Object);

            // Act.
            var result = _service.GetCandidateExerciseList(applicationUserId: Guid.NewGuid());

            // Assert.
            Assert.AreEqual(2, result.Count());
        }
    }
}
