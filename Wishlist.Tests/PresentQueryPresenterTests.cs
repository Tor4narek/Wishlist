using System;
using System.Collections.Generic;
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
    public class PresentQueryPresenterTests
    {
        private Mock<IPresentRepository> _presentRepositoryMock;
        private PresentQueryPresenter _presentQueryPresenter;
        private CancellationToken _cancellationToken;

        [SetUp]
        public void SetUp()
        {
            _presentRepositoryMock = new Mock<IPresentRepository>();
            _presentQueryPresenter = new PresentQueryPresenter(_presentRepositoryMock.Object);
            _cancellationToken = new CancellationToken();
        }

        [Test]
        public async Task LoadWishlistPresentsAsync_ShouldCallRepositoryGetPresents_WhenValidWishlistIdProvided()
        {
            // Arrange
            string wishlistId = "test_wishlist_id";
            var expectedPresents = new List<Present>
            {
                new Present(Guid.NewGuid(), "Present 1", "Description 1", wishlistId, false, null),
                new Present(Guid.NewGuid(), "Present 2", "Description 2", wishlistId, false, null)
            };
            _presentRepositoryMock.Setup(repo => repo.GetPresentsAsync(wishlistId, _cancellationToken)).ReturnsAsync(expectedPresents);

            // Act
            var presents = await _presentQueryPresenter.LoadWishlistPresentsAsync(wishlistId, _cancellationToken);

            // Assert
            Assert.AreEqual(expectedPresents, presents);
            _presentRepositoryMock.Verify(repo => repo.GetPresentsAsync(wishlistId, _cancellationToken), Times.Once);
        }

        [Test]
        public void LoadWishlistPresentsAsync_ShouldThrowArgumentNullException_WhenWishlistIdIsNullOrEmpty()
        {
            // Arrange
            string wishlistId = null;

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _presentQueryPresenter.LoadWishlistPresentsAsync(wishlistId, _cancellationToken));

            Assert.That(ex.ParamName, Is.EqualTo("wishlistId"));
        }

        [Test]
        public async Task SearchPresentsByKeywordAsync_ShouldCallRepositorySearchPresents_WhenValidKeywordProvided()
        {
            // Arrange
            string keyword = "test_keyword";
            var expectedPresents = new List<Present>
            {
                new Present(Guid.NewGuid(), "Keyword Present", "Description", "wishlistId", false, null)
            };
            _presentRepositoryMock.Setup(repo => repo.SearchPresentsByKeywordAsync(keyword, _cancellationToken)).ReturnsAsync(expectedPresents);

            // Act
            var presents = await _presentQueryPresenter.SearchPresentsByKeywordAsync(keyword, _cancellationToken);

            // Assert
            Assert.AreEqual(expectedPresents, presents);
            _presentRepositoryMock.Verify(repo => repo.SearchPresentsByKeywordAsync(keyword, _cancellationToken), Times.Once);
        }

        [Test]
        public void SearchPresentsByKeywordAsync_ShouldThrowArgumentNullException_WhenKeywordIsNullOrEmpty()
        {
            // Arrange
            string keyword = "";

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _presentQueryPresenter.SearchPresentsByKeywordAsync(keyword, _cancellationToken));

            Assert.That(ex.ParamName, Is.EqualTo("keyword"));
        }

        [Test]
        public async Task LoadReservedPresentsAsync_ShouldCallRepositoryGetReservedPresents_WhenValidUserIdProvided()
        {
            // Arrange
            string userId = "test_user_id";
            var expectedReservedPresents = new List<Present>
            {
                new Present(Guid.NewGuid(), "Reserved Present", "Description", "wishlistId", true, userId)
            };
            _presentRepositoryMock.Setup(repo => repo.GetReservedPresentsAsync(userId, _cancellationToken)).ReturnsAsync(expectedReservedPresents);

            // Act
            var reservedPresents = await _presentQueryPresenter.LoadReservedPresentsAsync(userId, _cancellationToken);

            // Assert
            Assert.AreEqual(expectedReservedPresents, reservedPresents);
            _presentRepositoryMock.Verify(repo => repo.GetReservedPresentsAsync(userId, _cancellationToken), Times.Once);
        }

        [Test]
        public void LoadReservedPresentsAsync_ShouldThrowArgumentNullException_WhenUserIdIsNullOrEmpty()
        {
            // Arrange
            string userId = "";

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _presentQueryPresenter.LoadReservedPresentsAsync(userId, _cancellationToken));

            Assert.That(ex.ParamName, Is.EqualTo("userId"));
        }
    }
}
