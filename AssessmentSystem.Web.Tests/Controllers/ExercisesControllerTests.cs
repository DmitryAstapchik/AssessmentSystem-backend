using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Hosting;
using System.Web.Http.Results;
using AssessmentSystem.Controllers;
using AssessmentSystem.Models.ExerciseManagement;
using AssessmentSystem.Services.ExerciseManagement;
using AssessmentSystem.Services.Security;
using Moq;
using NUnit.Framework;

namespace AssessmentSystem.Web.Tests.Controllers
{
    [TestFixture]
    internal class ExercisesControllerTests
    {
        private ExercisesController _exerciseController;

        [SetUp]
        public void Init()
        {
            var request = new HttpRequestMessage();
            request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            request.Method = HttpMethod.Get;

            var mockUserContext = new Mock<IUserContext>();
            mockUserContext.SetupGet(t => t.Id).Returns(Guid.Empty);

            var mockExerciseService = new Mock<IExerciseService>();
            mockExerciseService.Setup(service => service.GetCandidateExerciseList(It.IsAny<Guid>()))
                .Returns(() => new List<ExerciseForList>());

            var mockUserContextService = new Mock<IUserContextService>();
            mockUserContextService.Setup(service => service.GetCurrentUserAsync())
                .Returns(() => System.Threading.Tasks.Task.Run(() => mockUserContext.Object));

            _exerciseController = new ExercisesController(
                mockExerciseService.Object, mockUserContextService.Object);
        }

        [Test]
        public void GetCandidateExercisesTest_ReturnNotNullArray()
        {
            var result = _exerciseController.GetCandidateExercises().Result;

            Assert.NotNull(result);
            Assert.IsInstanceOf<OkNegotiatedContentResult<ExerciseForList[]>>(result);
        }
    }
}