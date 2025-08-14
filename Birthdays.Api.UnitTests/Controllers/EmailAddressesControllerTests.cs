using Birthdays.Api.Controllers;
using Birthdays.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using static NUnit.Framework.Assert;

namespace Birthdays.Api.UnitTests.Controllers;

[TestFixture]
public class EmailAddressesControllerTests
{
    private Mock<IEmailAddressesService> _emailAddressesServiceMock;
    private EmailAddressesController _controller;

    [SetUp]
    public void Setup()
    {
        _emailAddressesServiceMock = new Mock<IEmailAddressesService>();
        _controller = new EmailAddressesController(_emailAddressesServiceMock.Object);
    }

    [Test]
    public async Task AddEmail_ValidEmail_CallsInsertEmailAddressAsync()
    {
        // Arrange
        const string validEmail = "test@example.com";

        // Act
        var result = await _controller.AddEmail(validEmail);

        // Assert
        _emailAddressesServiceMock.Verify(
            service => service.InsertEmailAddressAsync(validEmail),
            Times.Once);
        That(result, Is.InstanceOf<OkResult>());
    }
    
    [Test]
    public async Task DeleteEmail_ValidEmail_CallsDeleteEmailAddressAsync()
    {
        // Arrange
        const string validEmail = "test@example.com";

        // Act
        var result = await _controller.DeleteEmail(validEmail);

        // Assert
        _emailAddressesServiceMock.Verify(
            service => service.DeleteEmailAddressAsync(validEmail),
            Times.Once);
        That(result, Is.InstanceOf<NoContentResult>());
    }
}
