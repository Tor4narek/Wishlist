using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Presenter;
using Repository;
using Models;
using Presenter.Services;

namespace Presenter.Tests
{
    [TestFixture]
    public class UserPresenterTests
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IAuthenticationService> _authServiceMock;
        private UserPresenter _userPresenter;
        private CancellationToken _cancellationToken;

        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _authServiceMock = new Mock<IAuthenticationService>();
            _userPresenter = new UserPresenter(_userRepositoryMock.Object, _authServiceMock.Object);
            _cancellationToken = new CancellationToken();
        }

        [Test]
        public async Task CreateUserAsync_ShouldCallRegisterUser()
        {
            // Arrange
            string name = "TestUser";
            string email = "test@example.com";
            string password = "password";

            // Act
            await _userPresenter.CreateUserAsync(name, email, password, _cancellationToken);

            // Assert
            _authServiceMock.Verify(auth => auth.RegisterUserAsync(name, email, password, _cancellationToken), Times.Once);
        }

        [Test]
        public void CreateUserAsync_ShouldThrowException_WhenRegistrationFails()
        {
            // Arrange
            _authServiceMock.Setup(auth => auth.RegisterUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Registration failed"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _userPresenter.CreateUserAsync("Test", "test@example.com", "password", _cancellationToken));

            Assert.That(ex.Message, Is.EqualTo("Error while creating user: Registration failed"));
        }

        [Test]
        public async Task AuthenticateUserAsync_ShouldReturnUser_WhenAuthenticationSucceeds()
        {
            // Arrange
            string email = "test@example.com";
            string password = "password";
            var expectedUser = new User("1", "TestUser", email, "hashedPassword");

            _authServiceMock.Setup(auth => auth.AuthenticateUserAsync(email, password, _cancellationToken))
                .Returns(Task.CompletedTask);
            _authServiceMock.Setup(auth => auth.GetAuthenticatedUserAsync())
                .ReturnsAsync(expectedUser);

            // Act
            var user = await _userPresenter.AuthenticateUserAsync(email, password, _cancellationToken);

            // Assert
            Assert.AreEqual(expectedUser, user);
        }

        [Test]
        public async Task LoadUserAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            string userId = "1";
            var expectedUser = new User(userId, "TestUser", "test@example.com", "hashedPassword");

            _userRepositoryMock.Setup(repo => repo.GetUserAsync(userId, _cancellationToken))
                .ReturnsAsync(expectedUser);

            // Act
            var user = await _userPresenter.LoadUserAsync(userId, _cancellationToken);

            // Assert
            Assert.AreEqual(expectedUser, user);
        }

        [Test]
        public void LoadUserAsync_ShouldThrowException_WhenUserNotFound()
        {
            // Arrange
            string userId = "1";
            _userRepositoryMock.Setup(repo => repo.GetUserAsync(userId, _cancellationToken))
                .ReturnsAsync((User)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _userPresenter.LoadUserAsync(userId, _cancellationToken));

            Assert.That(ex.Message, Is.EqualTo("Error while loading user: User not found."));
        }

       

        [Test]
        public async Task SearchUsersByKeywordAsync_ShouldReturnUsers_WhenKeywordMatches()
        {
            // Arrange
            string keyword = "Test";
            var expectedUsers = new List<User>
            {
                new User("1", "TestUser", "test@example.com", "hashedPassword")
            };

            _userRepositoryMock.Setup(repo => repo.SearchUsersByKeywordAsync(keyword, _cancellationToken))
                .ReturnsAsync(expectedUsers);

            // Act
            var users = await _userPresenter.SearchUsersByKeywordAsync(keyword, _cancellationToken);

            // Assert
            Assert.AreEqual(expectedUsers, users);
        }

        [Test]
        public async Task GetUserByEmailAsync_ShouldReturnUser_WhenEmailMatches()
        {
            // Arrange
            string email = "test@example.com";
            var expectedUser = new User("1", "TestUser", email, "hashedPassword");

            _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(email, _cancellationToken))
                .ReturnsAsync(expectedUser);

            // Act
            var user = await _userPresenter.GetUserByEmailAsync(email, _cancellationToken);

            // Assert
            Assert.AreEqual(expectedUser, user);
        }

        [Test]
        public async Task LogoutAsync_ShouldCallLogoutService()
        {
            // Act
            await _userPresenter.LogoutAsync();

            // Assert
            _authServiceMock.Verify(auth => auth.LogoutAsync(), Times.Once);
        }
    }
}
