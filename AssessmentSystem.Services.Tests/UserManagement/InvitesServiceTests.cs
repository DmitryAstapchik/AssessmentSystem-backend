using System;
using System.Collections.Generic;
using System.Linq;
using AssessmentSystem.Data.Access.Context.Interfaces;
using AssessmentSystem.Data.Access.UserManagement;
using AssessmentSystem.Models.UserManagement;
using AssessmentSystem.Services.Exceptions;
using Moq;
using NUnit.Framework;

namespace AssessmentSystem.Services.UserManagement
{
    [TestFixture]
    internal class InvitesServiceTests
    {
        private const string TestRoleId = "roleId";
        private const string TestInviteLink = "link";
        private const string TestEmail = "valid@email.com";
        private const string RoleName = "role";
        private InvitesService _service;
        private IEmailSendingService _emailService;
        private Mock<IInvitesContext> _contextMock;
        private FakeEntitySet<Invite> _fakeSet;

        [OneTimeSetUp]
        public void TestsSetUp()
        {
            _contextMock = new Mock<IInvitesContext>();
            _fakeSet = new FakeEntitySet<Invite>(new List<Invite>());
            _contextMock.SetupGet(c => c.Invites).Returns(_fakeSet);
            _emailService = Mock.Of<IEmailSendingService>();
            _service = new InvitesService(_contextMock.Object, _emailService);
        }

        [TearDown]
        public void Cleanup()
        {
            _fakeSet = new FakeEntitySet<Invite>(new List<Invite>());
            _contextMock.SetupGet(c => c.Invites).Returns(_fakeSet);
        }

        [Test]
        public void Ctor_NullArgs_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new InvitesService(null, _emailService));
            Assert.Throws<ArgumentNullException>(() => new InvitesService(_contextMock.Object, null));
        }

        [TestCase(null, RoleName)]
        [TestCase(TestEmail, null)]
        public void CreateInvite_NullArgs_ThrowsException(string email, string role)
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateInviteAsync(null));
            Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateInviteAsync(new InviteInfo(email, role)));
        }

        [Test]
        public void CreateInvite_InvalidArgs_ThrowsException()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _service.CreateInviteAsync(new InviteInfo(string.Empty, RoleName)));
            Assert.ThrowsAsync<FormatException>(() => _service.CreateInviteAsync(new InviteInfo("invalidEmail", RoleName)));
        }

        [Test]
        public void CreateInvite_NewInvite_AddsInviteToSetAndSavesChanges()
        {
            var count = _fakeSet.Count();

            _service.CreateInviteAsync(new InviteInfo(TestEmail, RoleName)).Wait();

            var invite = _fakeSet.Last();
            Assert.Multiple(() =>
            {
                Assert.True(_fakeSet.Count() == count + 1);
                Assert.True(invite.Email == TestEmail.ToLower());
                Assert.True(invite.RoleName == RoleName);
            });
            _contextMock.Verify(c => c.SaveChangesAsync());
        }

        [Test]
        public void CreateInvite_RepeatedInvite_UpdatesExistent()
        {
            var invite = new Invite { RoleName = RoleName, Email = TestEmail, ExpiredDate = DateTime.Now.AddDays(-1) };
            _fakeSet.Add(invite);
            var token = invite.Token;

            _service.CreateInviteAsync(new InviteInfo(TestEmail, RoleName)).Wait();

            Assert.Multiple(() =>
            {
                Assert.True(invite.Token != token);
                Assert.True(invite.ExpiredDate > DateTime.Now);
                Assert.DoesNotThrow(() => _fakeSet.Single(i => i.Email == TestEmail && i.RoleName == RoleName));
            });
        }

        [Test]
        public void SendInvite_NullInviteLink_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _service.SendInviteAsync(1, null));
        }

        [Test]
        public void SendInvite_AbsentInviteId_ThrowsException()
        {
            Assert.Throws<InviteNotFoundException>(() => _service.SendInviteAsync(1, TestInviteLink));
        }

        [Test]
        public void SendInvite_ExpiredInviteId_ThrowsException()
        {
            _fakeSet.Add(new Invite { Id = 1, ExpiredDate = DateTime.Now.AddDays(-1) });

            Assert.Throws<InvalidOperationException>(() => _service.SendInviteAsync(1, TestInviteLink));
        }

        [Test]
        public void GetUnusedInvites_ReturnsAllUnusedInvites()
        {
            string emailString = "email";
            var email1 = "email1";
            var email2 = "email2";
            var email3 = "email3";
            _fakeSet.Add(new Invite { Email = email1, RoleName = RoleName, UserId = null });
            _fakeSet.Add(new Invite { Email = TestEmail, RoleName = RoleName, UserId = "user1" });
            _fakeSet.Add(new Invite { Email = email2, RoleName = RoleName, UserId = null });
            _fakeSet.Add(new Invite { Email = TestEmail, RoleName = RoleName, UserId = "user2" });
            _fakeSet.Add(new Invite { Email = email3, RoleName = RoleName, UserId = null });

            var invites = _service.GetUnusedInvites(emailString);

            CollectionAssert.AreEquivalent(new[] { email1, email2, email3 }, invites.Select(i => i.Email));
        }

        [Test]
        public void GetUnusedInvites_ReturnsOrderedByEmailInvites()
        {
            string emailString = "email";
            var email1 = "Cemail1@gmail.com";
            var email2 = "Aemail2@gmail.com";
            var email3 = "Bemail3@gmail.com";
            _fakeSet.Add(new Invite { Email = email1, RoleName = RoleName, UserId = null });
            _fakeSet.Add(new Invite { Email = TestEmail, RoleName = RoleName, UserId = "user1" });
            _fakeSet.Add(new Invite { Email = email2, RoleName = RoleName, UserId = null });
            _fakeSet.Add(new Invite { Email = TestEmail, RoleName = RoleName, UserId = "user2" });
            _fakeSet.Add(new Invite { Email = email3, RoleName = RoleName, UserId = null });

            var invites = _service.GetUnusedInvites(emailString);

            Assert.That(invites, Is.Ordered.By("Email"));
        }

        [Test]
        [TestCase("", ExpectedResult = new string[0])]
        [TestCase("    ", ExpectedResult = new string[0])]
        [TestCase("g", ExpectedResult = new[] { "email1@gmail.com", "email2@gmail.com", "email3@gmail.rom" })]
        [TestCase("G", ExpectedResult = new[] { "email1@gmail.com", "email2@gmail.com", "email3@gmail.rom" })]
        [TestCase("com", ExpectedResult = new[] { "email1@gmail.com", "email2@gmail.com" })]
        public IEnumerable<string> GetUnusedInvites_EmailString_ReturnsAllEligibleInvites(string email)
        {
            var email1 = "email1@gmail.com";
            var email2 = "email2@gmail.com";
            var email3 = "email3@gmail.rom";
            _fakeSet.Add(new Invite { Email = email1, RoleName = RoleName, UserId = null });
            _fakeSet.Add(new Invite { Email = TestEmail, RoleName = RoleName, UserId = "user1" });
            _fakeSet.Add(new Invite { Email = email2, RoleName = RoleName, UserId = null });
            _fakeSet.Add(new Invite { Email = TestEmail, RoleName = RoleName, UserId = "user2" });
            _fakeSet.Add(new Invite { Email = email3, RoleName = RoleName, UserId = null });

            var invites = _service.GetUnusedInvites(email);

            return invites.Select(x => x.Email);
        }

        [Test]
        public void FindInviteByToken_ExistingToken_ReturnsInviteInfo()
        {
            var token = Guid.NewGuid();
            var expectedInvite = new InviteInfo(TestEmail, RoleName);
            _fakeSet.Add(new Invite { Email = TestEmail, RoleName = RoleName, Token = token });

            var actualInvite = _service.FindInviteByToken(token);

            Assert.True(expectedInvite.Email == actualInvite.Email && expectedInvite.RoleName == actualInvite.RoleName);
        }

        [Test]
        public void FindInviteByToken_NonexistentToken_ReturnsNull()
        {
            var token = Guid.NewGuid();

            var actualInvite = _service.FindInviteByToken(token);

            Assert.IsNull(actualInvite);
        }

        [Test]
        public void UseInviteAsync_ExistingInvite_SetsUserIdAndMakesExpired()
        {
            var token = Guid.NewGuid();
            string userId = "userId";
            var invite = new Invite { Email = TestEmail, RoleName = RoleName, Token = token, ExpiredDate = DateTime.Now.AddDays(2), UserId = null };
            _fakeSet.Add(invite);

            _service.UseInviteAsync(token, userId).Wait();

            Assert.Multiple(() =>
            {
                Assert.AreEqual(userId, invite.UserId);
                Assert.True(invite.ExpiredDate <= DateTime.Now);
            });
        }

        [Test]
        public void UseInviteAsync_NonexistentInvite_ThrowsInviteNotFoundException()
        {
            Assert.ThrowsAsync<InviteNotFoundException>(() => _service.UseInviteAsync(Guid.NewGuid(), "userId"));
        }

        [Test]
        public void IsExpired_NonexistentInvite_ThrowsInviteNotFoundException()
        {
            Assert.Throws<InviteNotFoundException>(() => _service.IsExpired(Guid.NewGuid()));
        }

        [Test]
        public void IsExpired_ExpiredToken_ReturnsTrue()
        {
            var token = Guid.NewGuid();
            _fakeSet.Add(new Invite { ExpiredDate = DateTime.Now.AddDays(-2), Token = token });

            var result = _service.IsExpired(token);

            Assert.IsTrue(result);
        }

        [Test]
        public void IsExpired_ActiveToken_ReturnsFalse()
        {
            var token = Guid.NewGuid();
            _fakeSet.Add(new Invite { ExpiredDate = DateTime.Now.AddDays(2), Token = token });

            var result = _service.IsExpired(token);

            Assert.IsFalse(result);
        }
    }
}
