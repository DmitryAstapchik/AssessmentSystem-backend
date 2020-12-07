using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using AssessmentSystem.Controllers;
using AssessmentSystem.Identity;
using AssessmentSystem.Models.UserManagement;
using AssessmentSystem.Services.Identity;
using AssessmentSystem.Services.Security;
using AssessmentSystem.Services.UserManagement;
using Microsoft.AspNet.Identity;
using Moq;
using NUnit.Framework;

namespace AssessmentSystem.Web.Tests.Controllers
{
    [TestFixture]
    internal class InvitesControllerTests
    {
        private const string ValidTestEmail = "valid@email.com";
        private const string InvalidTestEmail = "invalidEmail";
        private const string CoachEmail = "coach@email.com";
        private const string ManagerEmail = "manager@email.com";
        private const string CoachRoleId = "CoachRoleId";
        private const string CoachRoleName = GlobalInfo.Coach;
        private const string CoachId = "coachId";
        private const int CoachInviteId = 1;
        private const int CoachInviteId1 = 2;
        private const int CoachInviteId2 = 3;
        private const string ManagerId = "managerId";
        private const string ManagerRoleId = "ManagerRoleId";
        private const string ManagerRoleName = GlobalInfo.Manager;
        private const int ManagerInviteId = 4;
        private readonly Guid _coachToken = Guid.NewGuid();
        private readonly Guid _managerToken1 = Guid.NewGuid();
        private readonly Guid _coachToken2 = Guid.NewGuid();
        private readonly Guid _coachToken3 = Guid.NewGuid();
        private readonly Guid _coachToken4 = Guid.NewGuid();
        private readonly Guid _managerToken = Guid.NewGuid();
        private InvitesController _controller;
        private Mock<IInvitesService> _invitesServiceMock;
        private Mock<ApplicationUserManager> _userManager;

        [OneTimeSetUp]
        public void Init()
        {
            _invitesServiceMock = new Mock<IInvitesService>();
            _invitesServiceMock.Setup(s =>
                s.CreateInviteAsync(It.Is<InviteInfo>(i =>
                    i.Email == ValidTestEmail && i.RoleName == CoachRoleName))).ReturnsAsync(CoachInviteId);
            _invitesServiceMock.Setup(s =>
                s.CreateInviteAsync(It.Is<InviteInfo>(i =>
                    i.Email == ValidTestEmail && i.RoleName == ManagerRoleName))).ReturnsAsync(ManagerInviteId);
            _invitesServiceMock.Setup(s => s.GetUnusedInvites(It.IsAny<string>())).Returns(new InviteInfo[0]);
            _invitesServiceMock.Setup(s => s.FindInviteByToken(_coachToken)).Returns(new InviteInfo() { Email = ValidTestEmail, RoleName = CoachRoleName });
            _invitesServiceMock.Setup(s => s.FindInviteByToken(_managerToken1)).Returns(new InviteInfo() { Email = CoachEmail, RoleName = ManagerRoleName });
            _invitesServiceMock.Setup(s => s.FindInviteByToken(_coachToken2)).Returns(new InviteInfo() { Email = "email", RoleName = CoachRoleName });
            _invitesServiceMock.Setup(s => s.FindInviteByToken(_coachToken3)).Returns(new InviteInfo() { Email = CoachEmail, RoleName = CoachRoleName });
            _invitesServiceMock.Setup(s => s.FindInviteByToken(_coachToken4)).Throws<ArgumentException>();
            _invitesServiceMock.Setup(s => s.IsExpired(_coachToken)).Returns(true);
            _invitesServiceMock.Setup(s => s.IsExpired(_managerToken)).Returns(false);

            var userRoleServiceMock = new Mock<IUserRoleService>();
            var coachRole = Mock.Of<IUserRole>(r => r.Id == CoachRoleId && r.Name == CoachRoleName);
            var managerRole = Mock.Of<IUserRole>(r => r.Id == ManagerRoleId && r.Name == ManagerRoleName);
            userRoleServiceMock.Setup(s => s.FindRoleByNameAsync(CoachRoleName)).ReturnsAsync(coachRole);
            userRoleServiceMock.Setup(s => s.FindRoleByNameAsync(ManagerRoleName)).ReturnsAsync(managerRole);

            _userManager = new Mock<ApplicationUserManager>(Mock.Of<IUserStore<ApplicationUser>>(), Mock.Of<ApplicationIdentityDbContext>());
            _userManager.Setup(m => m.FindByEmailAsync(CoachEmail)).ReturnsAsync(new ApplicationUser() { Id = CoachId });
            _userManager.Setup(m => m.FindByEmailAsync(ValidTestEmail)).ReturnsAsync((ApplicationUser)null);
            _userManager.Setup(m => m.IsInRoleAsync(CoachId, CoachRoleName)).ReturnsAsync(true);
            _userManager.Setup(m => m.FindByEmailAsync(ManagerEmail)).ReturnsAsync(new ApplicationUser() { Id = ManagerId });
            _userManager.Setup(m => m.IsInRoleAsync(ManagerId, ManagerRoleName)).ReturnsAsync(true);
            _userManager.Setup(m => m.IsInRoleAsync(CoachId, ManagerRoleName)).ReturnsAsync(false);
            _userManager.Setup(m => m.IsInRoleAsync(CoachId, ManagerRoleName)).ReturnsAsync(false);

            _controller = new InvitesController(_invitesServiceMock.Object, userRoleServiceMock.Object, _userManager.Object)
            {
                Request = new HttpRequestMessage() { RequestUri = new Uri("http://labtester.com/invites") },
                Configuration = new HttpConfiguration(),
            };
            _controller.Configuration.Routes.MapHttpRoute(name: "RedirectInvite", routeTemplate: null);

            ConfigurationManager.AppSettings.Set("FrontendHost", "http://labtester.com/");
        }

        [TearDown]
        public void CleanUp()
        {
            _invitesServiceMock.ResetCalls();
            _userManager.ResetCalls();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        [TestCase(InvalidTestEmail)]
        public void SendInviteToCoach_InvalidEmail_ReturnsBadRequest(string invalidEmail)
        {
            var email = invalidEmail;

            var actionResult = _controller.SendInviteToCoach(email).Result;

            Assert.IsInstanceOf<BadRequestErrorMessageResult>(actionResult);
        }

        [Test]
        public void SendInviteToCoach_CoachEmail_ReturnsConflict()
        {
            var email = CoachEmail;

            var actionResult = _controller.SendInviteToCoach(email).Result;

            Assert.IsInstanceOf<ConflictResult>(actionResult);
        }

        [Test]
        public void SendInviteToCoach_ValidEmail_CreatesAnInvite()
        {
            var email = ValidTestEmail;

            var actionResult = _controller.SendInviteToCoach(email).Result;

            _invitesServiceMock.Verify(
                s => s.CreateInviteAsync(It.Is<InviteInfo>(
                    i => i.Email == ValidTestEmail && i.RoleName == CoachRoleName)), Times.Once);
        }

        [Test]
        public void SendInviteToCoach_ValidEmail_SendsAnInvite()
        {
            var email = ValidTestEmail;

            var actionResult = _controller.SendInviteToCoach(email).Result;

            _invitesServiceMock.Verify(s => s.SendInviteAsync(CoachInviteId, It.IsNotNull<string>()), Times.Once);
        }

        [Test]
        public void SendInviteToCoach_ValidEmail_ReturnsStatusOK()
        {
            var email = ValidTestEmail;

            var actionResult = _controller.SendInviteToCoach(email).Result;

            Assert.IsInstanceOf<OkResult>(actionResult);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        [TestCase(InvalidTestEmail)]
        public void SendInviteToManager_InvalidEmail_ReturnsBadRequest(string invalidEmail)
        {
            var email = invalidEmail;

            var actionResult = _controller.SendInviteToManager(email).Result;

            Assert.IsInstanceOf<BadRequestErrorMessageResult>(actionResult);
        }

        [Test]
        public void SendInviteToManager_ManagerEmail_ReturnsConflict()
        {
            var email = ManagerEmail;

            var actionResult = _controller.SendInviteToManager(email).Result;

            Assert.IsInstanceOf<ConflictResult>(actionResult);
        }

        [Test]
        public void SendInviteToManager_ValidEmail_CreatesAnInvite()
        {
            var email = ValidTestEmail;

            var actionResult = _controller.SendInviteToManager(email).Result;

            _invitesServiceMock.Verify(s => s.CreateInviteAsync(It.Is<InviteInfo>(i => i.Email == ValidTestEmail && i.RoleName == ManagerRoleName)), Times.Once);
        }

        [Test]
        public void SendInviteToManager_ValidEmail_SendsAnInvite()
        {
            var email = ValidTestEmail;

            var actionResult = _controller.SendInviteToManager(email).Result;

            _invitesServiceMock.Verify(s => s.SendInviteAsync(ManagerInviteId, It.IsNotNull<string>()), Times.Once);
        }

        [Test]
        public void SendInviteToManager_ValidEmail_ReturnsStatusOK()
        {
            var email = ValidTestEmail;

            var actionResult = _controller.SendInviteToManager(email).Result;

            Assert.IsInstanceOf<OkResult>(actionResult);
        }

        [Test]
        public void GetUnusedInvites_ReturnsOkWithInvites()
        {
            var actionResult = _controller.GetUnusedInvites("email");

            Assert.IsInstanceOf<OkNegotiatedContentResult<IEnumerable<InviteInfo>>>(actionResult);
        }

        [Test]
        public void VerifyInvite_NonexistentToken_ReturnsRedirectToNotFound()
        {
            var token = Guid.NewGuid();

            var actionResult = (RedirectResult)_controller.VerifyInviteAsync(token).Result;

            Assert.AreEqual(GlobalInfo.InviteNotFoundUrl, actionResult.Location.AbsoluteUri);
        }

        [Test]
        public void VerifyInvite_ExpiredInvite_ReturnsRedirectToExpired()
        {
            var token = _coachToken;

            var actionResult = (RedirectResult)_controller.VerifyInviteAsync(token).Result;

            Assert.AreEqual(GlobalInfo.InviteIsExpiredUrl, actionResult.Location.AbsoluteUri);
        }

        [Test]
        public void VerifyInvite_ExistingUser_ReturnsRedirectToRoleAdded()
        {
            var token = _managerToken1;

            var actionResult = (RedirectResult)_controller.VerifyInviteAsync(token).Result;

            Assert.AreEqual(GlobalInfo.UserAddedToRoleUrl, actionResult.Location.AbsoluteUri);
        }

        [Test]
        public void VerifyInvite_NonexistentUser_ReturnsRedirectToRegister()
        {
            var token = _coachToken2;

            var actionResult = (RedirectResult)_controller.VerifyInviteAsync(token).Result;

            Assert.AreEqual(GlobalInfo.RegisterByInviteUrl + token, actionResult.Location.AbsoluteUri);
        }

        [Test]
        public void VerifyInvite_ExistingUserInRole_ReturnsRedirectToAlreadyInRole()
        {
            var token = _coachToken3;

            var actionResult = (RedirectResult)_controller.VerifyInviteAsync(token).Result;

            Assert.AreEqual(GlobalInfo.UserAlreadyInRoleUrl, actionResult.Location.AbsoluteUri);
        }

        [Test]
        public void VerifyInvite_Exception_ReturnsRedirectToInviteError()
        {
            var token = _coachToken4;

            var actionResult = (RedirectResult)_controller.VerifyInviteAsync(token).Result;

            Assert.AreEqual(GlobalInfo.InviteError, actionResult.Location.AbsoluteUri);
        }

        [Test]
        public void VerifyInvite_ExistingUser_AddsToRoleAndUsesInvite()
        {
            var token = _managerToken1;

            var actionResult = (RedirectResult)_controller.VerifyInviteAsync(token).Result;

            _userManager.Verify(s => s.AddToRoleAsync(CoachId, ManagerRoleName), Times.Once);
            _invitesServiceMock.Verify(s => s.UseInviteAsync(token, CoachId));
        }

        [Test]
        public void GetActiveInvite_NonexistentInvite_ReturnsNotFound()
        {
            var token = Guid.NewGuid();

            var actionResult = _controller.GetActiveInvite(token);

            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }

        [Test]
        public void GetActiveInvite_ExpiredInvite_ReturnsNotFound()
        {
            var token = _coachToken;

            var actionResult = _controller.GetActiveInvite(token);

            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }

        [Test]
        public void GetActiveInvite_ActiveToken_ReturnsOkWhithInviteInfo()
        {
            var token = _managerToken1;

            var actionResult = _controller.GetActiveInvite(token);

            Assert.IsInstanceOf<OkNegotiatedContentResult<InviteInfo>>(actionResult);
        }
    }
}
