using System;
using System.Configuration;
using System.Net.Mail;
using AssessmentSystem.Controllers;
using AssessmentSystem.Identity;
using AssessmentSystem.Models.Security;
using AssessmentSystem.Services.Identity;
using AssessmentSystem.Services.UserManagement;
using Microsoft.AspNet.Identity;
using Moq;
using NUnit.Framework;

namespace AssessmentSystem.Web.Tests.Controllers
{
    [TestFixture]
    internal class AccountsControllerTests
    {
        private static RegisterAccountRequest _registerAccount = new RegisterAccountRequest
        {
            Email = "dan.boy@mail.ru",
            FirstName = "Dan",
            LastName = "Boy",
            Password = "asdfty123",
            ConfirmPassword = "asdfty123"
        };

        private static ApplicationUser _registerUser = new ApplicationUser
        {
            DomainId = Guid.NewGuid(),
            UserName = _registerAccount.Email,
            Email = _registerAccount.Email,
            IsActive = true,
            Profile = new ApplicationUserProfile { FirstName = _registerAccount.FirstName, LastName = _registerAccount.LastName }
        };

        private Mock<IUserStore<ApplicationUser>> _userStoreMock;
        private Mock<ApplicationIdentityDbContext> _dbContextMock;
        private Mock<ApplicationUserManager> _applicationUserManager;
        private Mock<IEmailSendingService> _emailSendingServiceMock;
        private Mock<IInvitesService> _inviteServiceMock;
        private ForgotPasswordRequest _testSendConfirmEmail = new ForgotPasswordRequest { Email = "dan.boy@mail.ru" };

        [OneTimeSetUp]
        public void Init()
        {
            _userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _dbContextMock = new Mock<ApplicationIdentityDbContext>();

            _applicationUserManager = new Mock<ApplicationUserManager>(_userStoreMock.Object, _dbContextMock.Object);
            _emailSendingServiceMock = new Mock<IEmailSendingService>();
            _inviteServiceMock = new Mock<IInvitesService>();

            ConfigurationManager.AppSettings.Set("FrontendHost", "http://labtester.com/");
        }

        [TearDown]
        public void CleanUp()
        {
            _applicationUserManager.Reset();
            _emailSendingServiceMock.Reset();
            _applicationUserManager.ResetCalls();
            _emailSendingServiceMock.ResetCalls();
        }

        [TestCase]
        public void Register_Call_FindByNameAsync_CreateAsync_AddToRoleAsync_GenerateEmailConfirmationTokenAsync_SendAsync_Methods()
        {
            var accountsController = new AccountsController(
                _applicationUserManager.Object,
                _emailSendingServiceMock.Object,
                _inviteServiceMock.Object);

            var registerResult = IdentityResult.Success;
            var code = "randomstring";

            _applicationUserManager.Setup(um => um.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);
            _applicationUserManager.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(registerResult);
            _applicationUserManager.Setup(um => um.GenerateEmailConfirmationTokenAsync(It.IsAny<string>())).ReturnsAsync(code);

            var actionResult = accountsController.Register(_registerAccount);

            _applicationUserManager.Verify(um => um.FindByNameAsync(It.IsAny<string>()), Times.Once());
            _applicationUserManager.Verify(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once());
            _applicationUserManager.Verify(um => um.GenerateEmailConfirmationTokenAsync(It.IsAny<string>()), Times.Once());
            _emailSendingServiceMock.Verify(es => es.SendAsync(It.IsAny<MailMessage>()), Times.Once());
            _applicationUserManager.Verify(um => um.AddToRoleAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }

        [TestCase]
        public void SendConfirmEmail_Call_FindByNameAsync_GenerateEmailConfirmationTokenAsync_SendAsync_Methods()
        {
            var accountsController = new AccountsController(
                _applicationUserManager.Object,
                _emailSendingServiceMock.Object,
                _inviteServiceMock.Object);
            var code = "randomstring";

            _applicationUserManager.Setup(um => um.FindByNameAsync(_testSendConfirmEmail.Email)).ReturnsAsync(_registerUser);
            _applicationUserManager.Setup(um => um.GenerateEmailConfirmationTokenAsync(It.IsAny<string>())).ReturnsAsync(code);

            var actionResult = accountsController.SendConfirmEmail(_testSendConfirmEmail);

            _applicationUserManager.Verify(um => um.FindByNameAsync(It.IsAny<string>()), Times.Once());
            _applicationUserManager.Verify(um => um.GenerateEmailConfirmationTokenAsync(It.IsAny<string>()), Times.Once());
            _emailSendingServiceMock.Verify(ess => ess.SendAsync(It.IsAny<MailMessage>()), Times.Once());
        }

        [TestCase]
        public void ConfirmEmail_Call_FindByIdAsync_ConfirmEmailAsync_Methods()
        {
            var accountsController = new AccountsController(
                _applicationUserManager.Object,
                _emailSendingServiceMock.Object,
                _inviteServiceMock.Object);

            var confirmEmail = new ConfirmEmailRequest
            {
                Code = "code",
                UserId = "userIdId"
            };
            var confirmResult = IdentityResult.Success;

            _applicationUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(_registerUser);
            _applicationUserManager.Setup(um => um.ConfirmEmailAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(confirmResult);

            var actionResult = accountsController.ConfirmEmail(confirmEmail);

            _applicationUserManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Once());
            _applicationUserManager.Verify(um => um.ConfirmEmailAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }

        [TestCase]
        public void ForgotPassword_Call_FindByNameAsync_GeneratePasswordResetTokenAsync_SendAsync_Methods()
        {
            var accountsController = new AccountsController(
                _applicationUserManager.Object,
                _emailSendingServiceMock.Object,
                _inviteServiceMock.Object);

            var forgotPassword = new ForgotPasswordRequest { Email = "dan.boy@mail.ru" };
            var code = "randomstring";

            _applicationUserManager.Setup(um => um.FindByNameAsync(_testSendConfirmEmail.Email)).ReturnsAsync(_registerUser);
            _applicationUserManager.Setup(um => um.GeneratePasswordResetTokenAsync(It.IsAny<string>())).ReturnsAsync(code);

            var actionResult = accountsController.ForgotPassword(forgotPassword);

            _applicationUserManager.Verify(um => um.FindByNameAsync(It.IsAny<string>()), Times.Once());
            _applicationUserManager.Verify(um => um.GeneratePasswordResetTokenAsync(It.IsAny<string>()), Times.Once());
            _emailSendingServiceMock.Verify(es => es.SendAsync(It.IsAny<MailMessage>()), Times.Once());
        }

        [TestCase]
        public void ResetPassword_Call_FindByIdAsync_ResetPasswordAsync_Methods()
        {
            var accountsController = new AccountsController(
                _applicationUserManager.Object,
                _emailSendingServiceMock.Object,
                _inviteServiceMock.Object);

            var resetPassword = new ResetPasswordRequest { UserId = "SomeIdId", Code = "Code" };
            var resetResult = IdentityResult.Success;

            _applicationUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(_registerUser);
            _applicationUserManager.Setup(um => um.ResetPasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(resetResult);

            var actionResult = accountsController.ResetPassword(resetPassword);

            _applicationUserManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Once());
            _applicationUserManager.Verify(um => um.ResetPasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }

        [TestCase]
        public void GetUserIdByEmail_CallFindByEmailAsyncMethod()
        {
            var accountsController = new AccountsController(
                _applicationUserManager.Object,
                _emailSendingServiceMock.Object,
                _inviteServiceMock.Object);

            var email = "dan.boy@mail.ru";

            var actionResult = accountsController.GetUserIdByEmail(email);

            _applicationUserManager.Verify(um => um.FindByEmailAsync(It.IsAny<string>()), Times.Once());
        }
    }
}
