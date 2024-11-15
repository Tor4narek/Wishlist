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
public class UserRepositoryTests
{
    private Mock<IFileRepository<User>> _fileRepositoryMock;
    private UserRepository _userRepository;
    private CancellationToken _cancellationToken;

    [SetUp]
    public void SetUp()
    {
        _fileRepositoryMock = new Mock<IFileRepository<User>>();
        _userRepository = new UserRepository(_fileRepositoryMock.Object);
        _cancellationToken = new CancellationToken();
    }

    [Test]
    public async Task GetUserAsync_ReturnsCorrectUser_WhenUserExists()
    {
        // Arrange
        var userId = "123";
        var expectedUser = new User(userId, "Test User", "test@example.com", "hashedpassword");

        // Настроим мок так, чтобы он возвращал список пользователей
        _fileRepositoryMock
            .Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User> { expectedUser });

        // Act
        var actualUser = await _userRepository.GetUserAsync(userId, _cancellationToken);

        // Assert
        Assert.AreEqual(expectedUser, actualUser);
    }

    [Test]
    public async Task SearchUsersByKeywordAsync_FiltersCorrectly()
    {
        // Arrange
        var keyword = "Test";
        var users = new List<User>
        {
            new User("1", "Test User", "test@example.com", "hashedpassword"),
            new User("2", "Another User", "another@example.com", "hashedpassword")
        };

        // Настроим мок, чтобы вернуть список пользователей
        _fileRepositoryMock
            .Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        // Act
        var result = await _userRepository.SearchUsersByKeywordAsync(keyword, _cancellationToken);

        // Assert
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Test User", result.First().Name);
    }

    [Test]
    public async Task AddUserAsync_CallsRepositoryAddAsync()
    {
        // Arrange
        var newUser = new User("123", "New User", "newuser@example.com", "hashedpassword");

        // Act
        await _userRepository.AddUserAsync(newUser, _cancellationToken);

        // Assert
        _fileRepositoryMock.Verify(repo => repo.AddAsync(newUser, _cancellationToken), Times.Once);
    }


    [Test]
    public async Task UpdateUserAsync_CallsRepositoryUpdateAsync()
    {
        // Arrange
        var updatedUser = new User("123", "Updated User", "updateduser@example.com", "newhashedpassword");

        // Act
        await _userRepository.UpdateUserAsync(updatedUser, _cancellationToken);

        // Assert
        _fileRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Predicate<User>>(), updatedUser, _cancellationToken), Times.Once);
    }
}
