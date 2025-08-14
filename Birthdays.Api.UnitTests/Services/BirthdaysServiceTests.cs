using Birthdays.Api.Models.Dtos;
using Birthdays.Api.Models.Entities;
using Birthdays.Api.Repositories;
using Birthdays.Api.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using static NUnit.Framework.Assert;

namespace Birthdays.Api.UnitTests.Services;

[TestFixture]
public class BirthdaysServiceTests
{
    private Mock<IBirthdaysRepository> _repositoryMock;
    private BirthdaysService _service;

    [SetUp]
    public void Setup()
    {
        _repositoryMock = new Mock<IBirthdaysRepository>();
        _service = new BirthdaysService(_repositoryMock.Object);
    }

    [Test]
    public async Task GetBirthdaysAsync_ReturnsAllBirthdays()
    {
        // Arrange
        var birthdays = new List<Birthday>
        {
            new() { Id = 1, FirstName = "Любовь", LastName = "Петрова", BirthDay = new DateOnly(2000, 1, 1) },
            new() { Id = 2, FirstName = "Иван", LastName = "Иванов", BirthDay = new DateOnly(2001, 2, 2) }
        };

        _repositoryMock.Setup(r => r.GetBirthdaysAsync()).ReturnsAsync(birthdays);

        // Act
        // Assert
        var result = await _service.GetBirthdaysAsync();
        var resultArray = result as BirthdayDto[] ?? result.ToArray();
        That(resultArray, Is.Not.Null);
        That(resultArray, Has.Length.EqualTo(2));
    }

    [Test]
    public async Task GetBirthdayAsync_ReturnsBirthday_WhenExists()
    {
        // Arrange
        var birthday = new Birthday { Id = 1, FirstName = "Евгений", LastName = "Захаров", BirthDay = new DateOnly(2001, 1, 1) };
        _repositoryMock.Setup(r => r.GetBirthdayAsync(1)).ReturnsAsync(birthday);

        // Act
        // Assert
        var result = await _service.GetBirthdayAsync(1);
        That(result, Is.Not.Null);
        That(result!.FirstName, Is.EqualTo("Евгений"));
    }

    [Test]
    public async Task GetBirthdayAsync_ReturnsNull_WhenNotExists()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetBirthdayAsync(999)).ReturnsAsync((Birthday?)null);

        // Act
        // Assert
        var result = await _service.GetBirthdayAsync(999);
        That(result, Is.Null);
    }

    [Test]
    public async Task InsertBirthdayAsync_InsertsAndReturnsNewBirthday()
    {
        // Arrange
        var createDto = new CreateBirthdayDto("Алина", "Сидорова", new DateOnly(2005, 3, 3));
        var entity = new Birthday
        {
            FirstName = createDto.FirstName, LastName = createDto.LastName, BirthDay = createDto.BirthDay
        };

        _repositoryMock.Setup(r => r.InsertBirthdayAsync(entity)).Returns(Task.CompletedTask);
        _repositoryMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

        // Act
        // Assert
        var result = await _service.InsertBirthdayAsync(createDto);
        That(result, Is.Not.Null);
        That(result.FirstName, Is.EqualTo("Алина"));
    }

    [Test]
    public async Task UpdateBirthdayAsync_UpdatesExistingBirthday()
    {
        // Arrange
        var updateDto = new UpdateBirthdayDto("Валерий", "Носов", new DateOnly(2005, 1, 1));

        _repositoryMock.Setup(r => r.UpdateBirthdayAsync(1, It.IsAny<Birthday>())).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

        // Act
        // Assert
        var result = await _service.UpdateBirthdayAsync(1, updateDto);
        That(result, Is.Not.Null);
        That(result!.FirstName, Is.EqualTo("Валерий"));
    }

    [Test]
    public async Task UpdateBirthdayAsync_ReturnsNull_WhenNotExists()
    {
        // Arrange
        _repositoryMock.Setup(r => r.UpdateBirthdayAsync(999, It.IsAny<Birthday>())).ReturnsAsync(false);

        // Act
        // Assert
        var result = await _service.UpdateBirthdayAsync(999, 
            new UpdateBirthdayDto("Иван", "Иванов", new DateOnly(2020, 12, 11)));
        That(result, Is.Null);
    }

    [Test]
    public async Task DeleteBirthdayAsync_DeletesBirthday()
    {
        // Arrange
        _repositoryMock.Setup(r => r.DeleteBirthdayAsync(1)).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteBirthdayAsync(1);

        // Assert
        _repositoryMock.Verify(r => r.DeleteBirthdayAsync(1), Times.Once);
    }

    [Test]
    public async Task UploadPhotoAsync_DoesNothing_WhenFileIsNull()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetBirthdayAsync(1)).ReturnsAsync(new Birthday
        {
            Id = 1, FirstName = "Иван", LastName = "Иванов"
        });

        // Act
        await _service.UploadPhotoAsync(1, null);

        // Assert
        _repositoryMock.Verify(r => r.UpdateBirthdayAsync(It.IsAny<int>(), It.IsAny<Birthday>()), Times.Never);
    }

    [Test]
    public async Task UploadPhotoAsync_DoesNothing_WhenFileSizeIsZero()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(0L);
        _repositoryMock.Setup(r => r.GetBirthdayAsync(1)).ReturnsAsync(new Birthday
        {
            Id = 1, FirstName = "Иван", LastName = "Иванов"
        });

        // Act
        await _service.UploadPhotoAsync(1, fileMock.Object);

        // Assert
        _repositoryMock.Verify(r => r.UpdateBirthdayAsync(It.IsAny<int>(), It.IsAny<Birthday>()), Times.Never);
    }

    [Test]
    public async Task UploadPhotoAsync_CreatesDirectoryIfNotExists()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.FileName).Returns("test.jpg");
        fileMock.Setup(f => f.Length).Returns(100L);
        using var stream = new MemoryStream();
        fileMock.Setup(f => f.OpenReadStream()).Returns(stream);

        var birthday = new Birthday
        {
            Id = 1, FirstName = "Иван", LastName = "Иванов"
        };
        _repositoryMock.Setup(r => r.GetBirthdayAsync(1)).ReturnsAsync(birthday);
        _repositoryMock.Setup(r => r.UpdateBirthdayAsync(1, birthday)).ReturnsAsync(true);

        // Act
        await _service.UploadPhotoAsync(1, fileMock.Object);

        // Assert
        _repositoryMock.Verify(r => r.UpdateBirthdayAsync(1, birthday), Times.Once);
        That(birthday.PhotoPath, Is.Not.Null);
        That(Directory.Exists(Path.GetDirectoryName(birthday.PhotoPath)), Is.True);
    }
}