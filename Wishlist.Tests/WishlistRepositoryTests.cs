using Moq;
using NUnit.Framework;
using Repository;
using Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace Repository.Tests;
[TestFixture]
public class WishlistRepositoryTests
{
    private Mock<IFileRepository<Wishlist>> _fileRepositoryMock;
    private WishlistRepository _wishlistRepository;
    private CancellationToken _cancellationToken;

    [SetUp]
    public void SetUp()
    {
        _fileRepositoryMock = new Mock<IFileRepository<Wishlist>>();
        _wishlistRepository = new WishlistRepository(_fileRepositoryMock.Object);
        _cancellationToken = new CancellationToken();
    }

    [Test]
    public async Task GetUserWishlistsAsync_ReturnsUserWishlists()
    {
        // Arrange
        var userId = "user123";
        var wishlists = new List<Wishlist>
        {
            new Wishlist("1", "Birthday Gifts", "Gifts for my birthday", userId, "5"),
            new Wishlist("2", "Christmas Gifts", "Gifts for Christmas", "anotherUser", "10"),
            new Wishlist("3", "Travel Wishlist", "Items I need for travel", userId, "3")
        };

        _fileRepositoryMock
            .Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(wishlists);

        // Act
        var result = await _wishlistRepository.GetUserWishlistsAsync(userId, _cancellationToken);

        // Assert
        Assert.AreEqual(2, result.Count); // Должно вернуть 2 вишлиста для данного пользователя
        Assert.IsTrue(result.Any(w => w.Name == "Birthday Gifts"));
        Assert.IsTrue(result.Any(w => w.Name == "Travel Wishlist"));
    }

    [Test]
    public void AddWishlistAsync_ThrowsArgumentNullException_WhenWishlistIsNull()
    {
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _wishlistRepository.AddWishlistAsync(null, _cancellationToken));
    }

    [Test]
    public async Task AddWishlistAsync_CallsRepositoryAddAsync()
    {
        // Arrange
        var newWishlist = new Wishlist("wishlist1", "New Wishlist", "Description", "user123", "0");

        // Act
        await _wishlistRepository.AddWishlistAsync(newWishlist, _cancellationToken);

        // Assert
        _fileRepositoryMock.Verify(repo => repo.AddAsync(newWishlist, _cancellationToken), Times.Once);
    }

    [Test]
    public async Task DeleteWishlistAsync_CallsRepositoryDeleteAsync()
    {
        // Arrange
        var wishlistId = "wishlist1";

        // Act
        await _wishlistRepository.DeleteWishlistAsync(wishlistId, _cancellationToken);

        // Assert
        _fileRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<Func<Wishlist, bool>>(), _cancellationToken), Times.Once);
    }

    [Test]
    public void UpdateWishlistAsync_ThrowsArgumentNullException_WhenWishlistIsNull()
    {
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _wishlistRepository.UpdateWishlistAsync(null, _cancellationToken));
    }

    [Test]
    public async Task UpdateWishlistAsync_CallsRepositoryUpdateAsync()
    {
        // Arrange
        var updatedWishlist = new Wishlist("wishlist1", "Updated Wishlist", "Updated Description", "user123", "7");

        // Act
        await _wishlistRepository.UpdateWishlistAsync(updatedWishlist, _cancellationToken);

        // Assert
        _fileRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Predicate<Wishlist>>(), updatedWishlist, _cancellationToken), Times.Once);
    }
}
