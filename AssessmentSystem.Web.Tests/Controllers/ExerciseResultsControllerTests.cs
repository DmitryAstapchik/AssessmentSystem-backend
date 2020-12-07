using System;
using System.Collections.Generic;
using System.Web.Http.Results;
using AssessmentSystem.Controllers;
using AssessmentSystem.Models.ExerciseExecutor;
using AssessmentSystem.Services.ExerciseExecutor;
using Moq;
using NUnit.Framework;

namespace AssessmentSystem.Web.Tests.Controllers
{
    [TestFixture]
    public class ExerciseResultsControllerTests
    {
        private readonly Guid _candidateId;
        private readonly ExerciseResultsController _controller;
        private readonly Mock<ICandidateExercisesResultsService> _serviceMock;

        public ExerciseResultsControllerTests()
        {
            _candidateId = Guid.NewGuid();
            _serviceMock = new Mock<ICandidateExercisesResultsService>();
            _controller = new ExerciseResultsController(_serviceMock.Object);
        }

        [TearDown]
        public void CleanUp()
        {
            _serviceMock.Reset();
        }

        [Test]
        public void Ctor_NullArgs_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new ExerciseResultsController(null));
        }

        [Test]
        public void GetCandidateExcerciseResults_ExistentId_ReturnsOkWithResults()
        {
            _serviceMock.Setup(s => s.GetCandidateExercisesResults(_candidateId)).Returns(new[] { new CandidateExerciseResult() });

            var results = _controller.GetCandidateExcerciseResults(_candidateId);

            Assert.IsInstanceOf<OkNegotiatedContentResult<IEnumerable<CandidateExerciseResult>>>(results);
        }

        [Test]
        public void GetCandidateExcerciseResults_AbsentId_ReturnsNotFound()
        {
            _serviceMock.Setup(s => s.GetCandidateExercisesResults(_candidateId)).Returns(new CandidateExerciseResult[0]);

            var results = _controller.GetCandidateExcerciseResults(_candidateId);

            Assert.IsInstanceOf<NotFoundResult>(results);
        }
    }
}
