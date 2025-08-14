using Birthdays.Api.Models.Entities;
using Birthdays.Api.Repositories;
using Birthdays.Api.Services;
using Moq;
using static NUnit.Framework.Assert;

namespace Birthdays.Api.UnitTests.Services;

[TestFixture]
public class EmailAddressesServiceTests
{
    private Mock<IEmailAddressesRepository> _repositoryMock;
    private EmailAddressesService _service;

    [SetUp]
    public void Setup()
    {
        _repositoryMock = new Mock<IEmailAddressesRepository>();
        _service = new EmailAddressesService(_repositoryMock.Object);
    }

    [Test]
    public async Task GetEmailAddressesAsync_ReturnsEmailAddressesFromRepository()
    {
        // Arrange
        var expectedEmails = new List<EmailAddress>
        {
            new() { Value = "test1@example.com" },
            new() { Value = "test2@example.com" }
        };

        _repositoryMock.Setup(repo => repo.GetEmailAddressesAsync())
                       .ReturnsAsync(expectedEmails);

        // Act
        var result = await _service.GetEmailAddressesAsync();

        // Assert
        That(result, Is.EqualTo(expectedEmails));
    }

    [Test]
    public async Task InsertEmailAddressAsync_ValidEmail_InsertsAndSaves()
    {
        // Arrange
        const string validEmail = "valid@example.com";

        // Act
        await _service.InsertEmailAddressAsync(validEmail);

        // Assert
        _repositoryMock.Verify(repo => repo.InsertEmailAddressAsync(validEmail), Times.Once);
        _repositoryMock.Verify(repo => repo.SaveAsync(), Times.Once);
    }

    [Test]
    public async Task InsertEmailAddressAsync_InvalidEmail_DoesNotInsertOrSave()
    {
        // Arrange
        const string invalidEmail = "invalid-email";

        // Act
        await _service.InsertEmailAddressAsync(invalidEmail);

        // Assert
        _repositoryMock.Verify(repo => repo.InsertEmailAddressAsync(It.IsAny<string>()), Times.Never);
        _repositoryMock.Verify(repo => repo.SaveAsync(), Times.Never);
    }

    [Test]
    public async Task DeleteEmailAddressAsync_DeletesSpecifiedEmail()
    {
        // Arrange
        const string emailToDelete = "delete@example.com";

        // Act
        await _service.DeleteEmailAddressAsync(emailToDelete);

        // Assert
        _repositoryMock.Verify(repo => repo.DeleteEmailAddressAsync(emailToDelete), Times.Once);
    }
}