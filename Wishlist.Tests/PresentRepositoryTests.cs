using Models;
using Moq;
using NUnit.Framework;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace Repository.Tests;
[TestFixture]
public class PresentRepositoryTests
{
    private Mock<IFileRepository<Present>> _fileRepositoryMock;
    private PresentRepository _presentRepository;
    private CancellationToken _cancellationToken;

    [SetUp]
    public void SetUp()
    {
        _fileRepositoryMock = new Mock<IFileRepository<Present>>();
        _presentRepository = new PresentRepository(_fileRepositoryMock.Object);
        _cancellationToken = new CancellationToken();
    }

    [Test]
    public async Task GetPresentsAsync_ReturnsPresentsForWishlist()
    {
        // Arrange
        var wishlistId = "wishlist123";
        var presents = new List<Present>
        {
            new Present(Guid.NewGuid(), "Toy", "A toy", wishlistId, false, null),
            new Present(Guid.NewGuid(), "Book", "A book", "anotherWishlist", false, null),
            new Present(Guid.NewGuid(), "Laptop", "A laptop", wishlistId, true, "user1")
        };

        _fileRepositoryMock
            .Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(presents);

        // Act
        var result = await _presentRepository.GetPresentsAsync(wishlistId, _cancellationToken);

        // Assert
        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.Any(p => p.Name == "Toy"));
        Assert.IsTrue(result.Any(p => p.Name == "Laptop"));
    }

    [Test]
    public void AddPresentAsync_ThrowsArgumentNullException_WhenPresentIsNull()
    {
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _presentRepository.AddPresentAsync(null, _cancellationToken));
    }

    [Test]
    public async Task AddPresentAsync_CallsRepositoryAddAsync()
    {
        // Arrange
        var newPresent = new Present(Guid.NewGuid(), "New Present", "Description", "wishlist123", false, null);

        // Act
        await _presentRepository.AddPresentAsync(newPresent, _cancellationToken);

        // Assert
        _fileRepositoryMock.Verify(repo => repo.AddAsync(newPresent, _cancellationToken), Times.Once);
    }

    [Test]
    public async Task DeletePresentAsync_CallsRepositoryDeleteAsync()
    {
        // Arrange
        var presentId = Guid.NewGuid();

        // Act
        await _presentRepository.DeletePresentAsync(presentId, _cancellationToken);

        // Assert
        _fileRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<Func<Present, bool>>(), _cancellationToken), Times.Once);
    }

    [Test]
    public async Task SearchPresentsByKeywordAsync_ReturnsFilteredPresents()
    {
        // Arrange
        var keyword = "Toy";
        var presents = new List<Present>
        {
            new Present(Guid.NewGuid(), "Toy Car", "A small car", "wishlist123", false, null),
            new Present(Guid.NewGuid(), "Laptop", "A gaming laptop", "wishlist123", false, null),
            new Present(Guid.NewGuid(), "Toy Train", "A toy train", "wishlist123", false, null)
        };

        _fileRepositoryMock
            .Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(presents);

        // Act
        var result = await _presentRepository.SearchPresentsByKeywordAsync(keyword, _cancellationToken);

        // Assert
        Assert.AreEqual(2, result.Count); // Должно найти два подарка с ключевым словом "Toy"
        Assert.IsTrue(result.Any(p => p.Name == "Toy Car"));
        Assert.IsTrue(result.Any(p => p.Name == "Toy Train"));
    }

    [Test]
    public void ReservePresentAsync_ThrowsException_WhenPresentIsNull()
    {
        // Arrange
        var presentId = Guid.NewGuid();

        _fileRepositoryMock
            .Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Present>());

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(() => _presentRepository.ReservePresentAsync(presentId, "user1", _cancellationToken));
        Assert.AreEqual("Подарок не найден", ex.Message);
    }

    [Test]
    public void ReservePresentAsync_ThrowsException_WhenPresentIsAlreadyReserved()
    {
        // Arrange
        var presentId = Guid.NewGuid();
        var present = new Present(presentId, "Laptop", "A gaming laptop", "wishlist123", true, "user1");

        _fileRepositoryMock
            .Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Present> { present });

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(() => _presentRepository.ReservePresentAsync(presentId, "user2", _cancellationToken));
        Assert.AreEqual("Подарок уже зарезервирован", ex.Message);
    }

    [Test]
    public async Task ReservePresentAsync_UpdatesPresentAsReserved()
    {
        // Arrange
        var presentId = Guid.NewGuid();
        var present = new Present(presentId, "Laptop", "A gaming laptop", "wishlist123", false, null);

        _fileRepositoryMock
            .Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Present> { present });

        // Act
        await _presentRepository.ReservePresentAsync(presentId, "user1", _cancellationToken);

        // Assert
        _fileRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Predicate<Present>>(), It.IsAny<Present>(), _cancellationToken), Times.Once);
    }

    [Test]
    public async Task GetReservedPresentsAsync_ReturnsPresentsReservedByUser()
    {
        // Arrange
        var userId = "user123";
        var presents = new List<Present>
        {
            new Present(Guid.NewGuid(), "Laptop", "A gaming laptop", "wishlist123", true, userId),
            new Present(Guid.NewGuid(), "Book", "A fantasy book", "wishlist123", false, null),
            new Present(Guid.NewGuid(), "Phone", "A new smartphone", "wishlist123", true, "anotherUser")
        };

        _fileRepositoryMock
            .Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(presents);

        // Act
        var result = await _presentRepository.GetReservedPresentsAsync(userId, _cancellationToken);

        // Assert
        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result.Any(p => p.Name == "Laptop"));
    }
}
