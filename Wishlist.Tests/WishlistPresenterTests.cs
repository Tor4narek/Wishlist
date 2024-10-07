using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Models;
using Presenter;
using Repository;

namespace Presenter.Tests
{
    [TestFixture]
    public class WishlistPresenterTests
    {
        private Mock<IWishlistRepository> _wishlistRepositoryMock;
        private Mock<IUserPresenter> _userPresenterMock;
        private WishlistPresenter _wishlistPresenter;
        private CancellationToken _cancellationToken;

        [SetUp]
        public void SetUp()
        {
            _wishlistRepositoryMock = new Mock<IWishlistRepository>();
            _userPresenterMock = new Mock<IUserPresenter>();
            _wishlistPresenter = new WishlistPresenter(_wishlistRepositoryMock.Object, _userPresenterMock.Object);
            _cancellationToken = new CancellationToken();
        }

        [Test]
        public async Task LoadUserWishlistsAsync_ShouldReturnWishlists_WhenUserExists()
        {
            // Arrange
            string userId = "1";
            var user = new User(userId, "TestUser", "test@example.com", "hashedPassword");
            var wishlists = new List<Wishlist>
            {
                new Wishlist("1", "Birthday", "My birthday wishlist", userId, "5")
            };

            _userPresenterMock.Setup(up => up.LoadUserAsync(userId, _cancellationToken)).ReturnsAsync(user);
            _wishlistRepositoryMock.Setup(repo => repo.GetUserWishlistsAsync(userId, _cancellationToken))
                                   .ReturnsAsync(wishlists);

            // Act
            var result = await _wishlistPresenter.LoadUserWishlistsAsync(userId, _cancellationToken);

            // Assert
            Assert.AreEqual(wishlists, result);
        }

        [Test]
        public void LoadUserWishlistsAsync_ShouldThrowArgumentException_WhenUserIdIsNullOrEmpty()
        {
            // Arrange
            string userId = "";

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _wishlistPresenter.LoadUserWishlistsAsync(userId, _cancellationToken));

            Assert.That(ex.Message, Is.EqualTo("User ID cannot be null or empty. (Parameter 'userId')"));
        }

        [Test]
        public void LoadUserWishlistsAsync_ShouldThrowArgumentException_WhenUserDoesNotExist()
        {
            // Arrange
            string userId = "1";
            _userPresenterMock.Setup(up => up.LoadUserAsync(userId, _cancellationToken)).ReturnsAsync((User)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _wishlistPresenter.LoadUserWishlistsAsync(userId, _cancellationToken));

            Assert.That(ex.Message, Is.EqualTo("User does not exist. (Parameter 'userId')"));
        }

        [Test]
        public async Task AddNewWishlistAsync_ShouldCallRepositoryAdd_WhenValidDataProvided()
        {
            // Arrange
            string name = "Birthday";
            string description = "Birthday gifts";
            string ownerId = "1";
            string presentsNumber = "5";

            // Act
            await _wishlistPresenter.AddNewWishlistAsync(name, description, ownerId, presentsNumber, _cancellationToken);

            // Assert
            _wishlistRepositoryMock.Verify(repo => repo.AddWishlistAsync(It.IsAny<Wishlist>(), _cancellationToken), Times.Once);
        }

        [Test]
        public void AddNewWishlistAsync_ShouldThrowArgumentException_WhenNameIsNullOrEmpty()
        {
            // Arrange
            string name = "";
            string description = "Description";
            string ownerId = "1";
            string presentsNumber = "5";

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _wishlistPresenter.AddNewWishlistAsync(name, description, ownerId, presentsNumber, _cancellationToken));

            Assert.That(ex.Message, Is.EqualTo("Name cannot be null or empty. (Parameter 'w_name')"));
        }

        [Test]
        public void AddNewWishlistAsync_ShouldThrowArgumentException_WhenOwnerIdIsNullOrEmpty()
        {
            // Arrange
            string name = "Birthday";
            string description = "Description";
            string ownerId = "";
            string presentsNumber = "5";

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _wishlistPresenter.AddNewWishlistAsync(name, description, ownerId, presentsNumber, _cancellationToken));

            Assert.That(ex.Message, Is.EqualTo("OwnerId cannot be null or empty. (Parameter 'w_ownerId')"));
        }

        [Test]
        public async Task DeleteWishlistAsync_ShouldCallRepositoryDelete_WhenWishlistIdIsValid()
        {
            // Arrange
            var wishlistId = Guid.NewGuid();

            // Act
            await _wishlistPresenter.DeleteWishlistAsync(wishlistId, _cancellationToken);

            // Assert
            _wishlistRepositoryMock.Verify(repo => repo.DeleteWishlistAsync(wishlistId.ToString(), _cancellationToken), Times.Once);
        }

        [Test]
        public void DeleteWishlistAsync_ShouldThrowArgumentException_WhenWishlistIdIsEmpty()
        {
            // Arrange
            var wishlistId = Guid.Empty;

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _wishlistPresenter.DeleteWishlistAsync(wishlistId, _cancellationToken));

            Assert.That(ex.Message, Is.EqualTo("Wishlist ID cannot be empty. (Parameter 'wishlistId')"));
        }

        [Test]
        public async Task UpdateWishlistAsync_ShouldCallRepositoryUpdate_WhenValidDataProvided()
        {
            // Arrange
            var wishlist = new Wishlist("1", "Birthday", "Birthday wishlist", "1", "5");
            string presentsNumber = "10";

            // Act
            await _wishlistPresenter.UpdateWishlistAsync(wishlist, presentsNumber, _cancellationToken);

            // Assert
            _wishlistRepositoryMock.Verify(repo => repo.UpdateWishlistAsync(It.IsAny<Wishlist>(), _cancellationToken), Times.Once);
        }

        [Test]
        public void UpdateWishlistAsync_ShouldThrowArgumentException_WhenPresentsNumberIsNullOrEmpty()
        {
            // Arrange
            var wishlist = new Wishlist("1", "Birthday", "Birthday wishlist", "1", "5");
            string presentsNumber = "";

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _wishlistPresenter.UpdateWishlistAsync(wishlist, presentsNumber, _cancellationToken));

            Assert.That(ex.Message, Is.EqualTo("Presents number cannot be null or empty. (Parameter 'w_presentsNumber')"));
        }
    }
}
