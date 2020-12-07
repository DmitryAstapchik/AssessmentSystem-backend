using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using AssessmentSystem.Identity;
using AssessmentSystem.Services.Identity;
using AssessmentSystem.Services.Security;
using Microsoft.AspNet.Identity;
using Moq;
using NUnit.Framework;

namespace AssessmentSystem.Web.Tests.Services
{
    [TestFixture]
    internal class ApplicationUserManagerTests
    {
        private ApplicationUserManager _userManager;

        [SetUp]
        public void Init()
        {
            var userProfilesDbSetMock = GetAsyncMockSet<ApplicationUserProfile>();
            var usersDbSetMock = GetAsyncMockSet<ApplicationUser>();
            var dbContextMock = new Mock<ApplicationIdentityDbContext>();
            dbContextMock.Setup(db => db.UserProfiles).Returns(userProfilesDbSetMock.Object);
            dbContextMock.Setup(db => db.Users).Returns(usersDbSetMock.Object);

            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();

            _userManager = new ApplicationUserManager(userStoreMock.Object, dbContextMock.Object);
        }

        [Test]
        public void GetProfiles_ZeroSet_ReturnsEmptyCollection()
        {
            // Act.
            var result = _userManager.GetProfiles(0, 0).ToArray();

            // Assert.
            Assert.AreEqual(0, result.Count());
        }

        [Test]
        public void GetProfileAsync_ThrowsException()
        {
            // Arrange.
            var userId = Guid.Empty;

            // Act.
            // Assert.
            Assert.That(
                async () => await _userManager.GetProfileAsync(userId), Throws.TypeOf<UserNotFoundException>());
        }

        [Test]
        public void GetAccountsAsync_ZeroSet_ReturnsEmptyCollection()
        {
            // Act.
            var result = _userManager.GetAccounts(0, 0).ToArray();

            // Assert.
            Assert.AreEqual(0, result.Count());
        }

        [Test]
        public void GetAccountAsync_ThrowsException()
        {
            // Arrange.
            var userId = Guid.Empty.ToString();

            // Act.
            // Assert.
            Assert.That(
                async () => await _userManager.GetAccountAsync(userId), Throws.TypeOf<ArgumentException>());
        }

        private static Mock<DbSet<T>> GetAsyncMockSet<T>()
            where T : class
        {
            var emptySet = new List<T>().AsQueryable();
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(emptySet.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(emptySet.ElementType);
            mockSet.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(new TestDbAsyncQueryProvider<T>(emptySet.Provider));
            mockSet.As<IDbAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator())
                .Returns(new TestDbAsyncEnumerator<T>(emptySet.GetEnumerator()));

            return mockSet;
        }
    }
}
