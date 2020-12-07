namespace AssessmentSystem.Services.ExerciseExecutor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AssessmentSystem.Data.Access.Context.Interfaces;
    using AssessmentSystem.Data.Access.ExerciseExecutor;
    using AutoMapper;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class CandidateExerciseResultServiceTests
    {
        private readonly Guid _candidateId = Guid.NewGuid();
        private CandidateExercisesResultsService _service;
        private IList<TaskResult> _taskResults = new List<TaskResult>();
        private IList<TestResult> _testResults = new List<TestResult>();

        [OneTimeSetUp]
        public void Init()
        {
            var fakeTaskResults = new FakeEntitySet<TaskResult>(_taskResults);
            var fakeTestResults = new FakeEntitySet<TestResult>(_testResults);
            var context = Mock.Of<IExercisesContext>(c => c.CandidateTaskResults == fakeTaskResults && c.CandidateTestResults == fakeTestResults);
            _service = new CandidateExercisesResultsService(context);
            Mapper.Initialize(cfg => cfg.AddProfile(typeof(Mapping.CandidateExerciseMappingProfile)));
        }

        [TearDown]
        public void Cleanup()
        {
            _taskResults.Clear();
            _testResults.Clear();
        }

        [Test]
        public void GetCandidateExercisesResults_CandidateId_ReturnsAllCandidatesTasksResults()
        {
            var result1 = new TaskResult() { Id = 1, CreatorId = _candidateId, IsCompleted = true };
            var result2 = new TaskResult() { Id = 2, CreatorId = Guid.NewGuid(), IsCompleted = true };
            var result3 = new TaskResult() { Id = 3, CreatorId = _candidateId, IsCompleted = true };
            _taskResults.Add(result1);
            _taskResults.Add(result2);
            _taskResults.Add(result3);

            var results = _service.GetCandidateExercisesResults(_candidateId).ToArray();

            CollectionAssert.AreEquivalent(new[] { result1.Id, result3.Id }, results.Select(r => r.Id));
        }

        [Test]
        public void GetCandidateExercisesResults_CandidateId_ReturnsAllCompletedTasksResults()
        {
            var result1 = new TaskResult() { Id = 1, CreatorId = _candidateId, IsCompleted = true };
            var result2 = new TaskResult() { Id = 2, CreatorId = _candidateId, IsCompleted = true };
            var result3 = new TaskResult() { Id = 3, CreatorId = _candidateId, IsCompleted = false };
            _taskResults.Add(result1);
            _taskResults.Add(result2);
            _taskResults.Add(result3);

            var results = _service.GetCandidateExercisesResults(_candidateId).ToArray();

            CollectionAssert.AreEquivalent(new[] { result1.Id, result2.Id }, results.Select(r => r.Id));
        }

        [Test]
        public void GetCandidateExercisesResults_CandidateId_ReturnsAllCandidatesTestsResults()
        {
            var result1 = new TestResult() { Id = 1, CreatorId = Guid.NewGuid() };
            var result2 = new TestResult() { Id = 2, CreatorId = _candidateId };
            var result3 = new TestResult() { Id = 3, CreatorId = _candidateId };
            _testResults.Add(result1);
            _testResults.Add(result2);
            _testResults.Add(result3);

            var results = _service.GetCandidateExercisesResults(_candidateId).ToArray();

            CollectionAssert.AreEquivalent(new[] { result2.Id, result3.Id }, results.Select(r => r.Id));
        }

        [Test]
        public void GetCandidateExercisesResults_CandidateId_ReturnsOrderedByScoreDescending()
        {
            var result1 = new TestResult() { Id = 1, CreatorId = _candidateId, Score = 3 };
            var result2 = new TestResult() { Id = 2, CreatorId = _candidateId, Score = 0 };
            var result3 = new TaskResult() { Id = 3, CreatorId = _candidateId, IsCompleted = true, Score = 9 };
            var result4 = new TaskResult() { Id = 4, CreatorId = _candidateId, IsCompleted = true, Score = 4 };
            _testResults.Add(result1);
            _testResults.Add(result2);
            _taskResults.Add(result3);
            _taskResults.Add(result4);

            var results = _service.GetCandidateExercisesResults(_candidateId).ToArray();

            Assert.That(results, Is.Ordered.Descending.By("Score"));
        }
    }
}
