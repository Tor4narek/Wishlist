using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Models;
using Repository;
using Presenter;

namespace Presenter.Tests
{
    [TestFixture]
    public class PresentCommandsPresenterTests
    {
        private Mock<IPresentRepository> _presentRepositoryMock;
        private PresentCommandsPresenter _presentCommandsPresenter;
        private CancellationToken _cancellationToken;

        [SetUp]
        public void SetUp()
        {
            _presentRepositoryMock = new Mock<IPresentRepository>();
            _presentCommandsPresenter = new PresentCommandsPresenter(_presentRepositoryMock.Object);
            _cancellationToken = new CancellationToken();
        }

        [Test]
        public async Task AddNewPresentAsync_ShouldCallRepositoryAdd_WhenValidDataProvided()
        {
            // Arrange
            string name = "New Present";
            string description = "Description for present";
            string reserverId = "123";
            string wishlistId = "456";

            // Act
            await _presentCommandsPresenter.AddNewPresentAsync(name, description, reserverId, wishlistId, _cancellationToken);

            // Assert
            _presentRepositoryMock.Verify(repo => repo.AddPresentAsync(It.IsAny<Present>(), _cancellationToken), Times.Once);
        }

        [Test]
        public void AddNewPresentAsync_ShouldThrowArgumentNullException_WhenNameIsNullOrEmpty()
        {
            // Arrange
            string name = "";
            string description = "Description";
            string reserverId = "123";
            string wishlistId = "456";

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _presentCommandsPresenter.AddNewPresentAsync(name, description, reserverId, wishlistId, _cancellationToken));

            Assert.That(ex.ParamName, Is.EqualTo("name"));
        }

        [Test]
        public void AddNewPresentAsync_ShouldThrowArgumentNullException_WhenDescriptionIsNullOrEmpty()
        {
            // Arrange
            string name = "Present";
            string description = "";
            string reserverId = "123";
            string wishlistId = "456";

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _presentCommandsPresenter.AddNewPresentAsync(name, description, reserverId, wishlistId, _cancellationToken));

            Assert.That(ex.ParamName, Is.EqualTo("description"));
        }

        [Test]
        public void AddNewPresentAsync_ShouldThrowArgumentNullException_WhenReserverIdIsNullOrEmpty()
        {
            // Arrange
            string name = "Present";
            string description = "Description";
            string reserverId = "";
            string wishlistId = "456";

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _presentCommandsPresenter.AddNewPresentAsync(name, description, reserverId, wishlistId, _cancellationToken));

            Assert.That(ex.ParamName, Is.EqualTo("reserverId"));
        }

        [Test]
        public void AddNewPresentAsync_ShouldThrowArgumentNullException_WhenWishlistIdIsNullOrEmpty()
        {
            // Arrange
            string name = "Present";
            string description = "Description";
            string reserverId = "123";
            string wishlistId = "";

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _presentCommandsPresenter.AddNewPresentAsync(name, description, reserverId, wishlistId, _cancellationToken));

            Assert.That(ex.ParamName, Is.EqualTo("wishlistId"));
        }


        [Test]
        public async Task ReservePresentAsync_ShouldCallRepositoryReserve_WhenValidDataProvided()
        {
            // Arrange
            var presentId = Guid.NewGuid().ToString();
            string reserverId = "123";

            // Act
            await _presentCommandsPresenter.ReservePresentAsync(presentId, reserverId, _cancellationToken);

            // Assert
            _presentRepositoryMock.Verify(repo => repo.ReservePresentAsync(presentId, reserverId, _cancellationToken), Times.Once);
        }

        [Test]
        public void ReservePresentAsync_ShouldThrowArgumentNullException_WhenReserverIdIsNullOrEmpty()
        {
            // Arrange
            var presentId = Guid.NewGuid().ToString();
            string reserverId = "";

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _presentCommandsPresenter.ReservePresentAsync(presentId, reserverId, _cancellationToken));

            Assert.That(ex.ParamName, Is.EqualTo("reserverId"));
        }
    }
}
