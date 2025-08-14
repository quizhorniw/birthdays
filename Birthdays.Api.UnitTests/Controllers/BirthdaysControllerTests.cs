using Birthdays.Api.Controllers;
using Birthdays.Api.Models.Dtos;
using Birthdays.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using static NUnit.Framework.Assert;

namespace Birthdays.Api.UnitTests.Controllers;

[TestFixture]
public class BirthdaysControllerTests
{
    private Mock<IBirthdaysService> _mockService;
    private BirthdaysController _controller;

    [SetUp]
    public void Setup()
    {
        _mockService = new Mock<IBirthdaysService>();
        _controller = new BirthdaysController(_mockService.Object);
    }

    [Test]
    public async Task GetBirthdays_ReturnsOkWithList()
    {
        // Arrange
        var mockData = new List<BirthdayDto>
        {
            new(1, "Иван", "Иванов", new DateOnly(2000, 1, 1), null),
            new(2, "Петр", "Петров", new DateOnly(2000, 2, 2), null)
        };
        _mockService.Setup(s => s.GetBirthdaysAsync()).ReturnsAsync(mockData);

        // Act
        // Assert
        var result = await _controller.GetBirthdays();
        That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        That(okResult!.Value, Is.EqualTo(mockData));
    }

    [Test]
    public async Task GetBirthday_ExistingId_ReturnsOk()
    {
        // Arrange
        const int id = 1;
        var mockData = new BirthdayDto(1, "Иван", "Иванов", new DateOnly(2000, 1, 1), null);
        _mockService.Setup(s => s.GetBirthdayAsync(id)).ReturnsAsync(mockData);

        // Act
        // Assert
        var result = await _controller.GetBirthday(id);
        That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        That(okResult!.Value, Is.EqualTo(mockData));
    }

    [Test]
    public async Task GetBirthday_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        const int id = 999;
        _mockService.Setup(s => s.GetBirthdayAsync(id)).ReturnsAsync((BirthdayDto)null);

        // Act
        var result = await _controller.GetBirthday(id);
        
        // Assert
        That(result.Result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task InsertBirthday_ValidModel_CallsServiceAndReturnsCreatedAtAction()
    {
        // Arrange
        var dto = new CreateBirthdayDto("Иван", "Иванов", new DateOnly(2000, 1, 1));
        var resultDto = new BirthdayDto(1, "Иван", "Иванов", new DateOnly(2000, 1, 1), null);
        _mockService.Setup(s => s.InsertBirthdayAsync(dto)).ReturnsAsync(resultDto);

        // Act
        // Assert
        var result = await _controller.InsertBirthday(dto);
        That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
        var actionResult = result.Result as CreatedAtActionResult;
        That(actionResult!.ActionName, Is.EqualTo("GetBirthday"));
        That(actionResult.RouteValues?["id"], Is.EqualTo(1));
        That(actionResult.Value, Is.EqualTo(resultDto));
    }

    [Test]
    public async Task UpdateBirthday_ExistingId_CallsServiceAndReturnsOk()
    {
        // Arrange
        const int id = 1;
        var dto = new UpdateBirthdayDto("Иван", "Иванов", new DateOnly(2000, 1, 1));
        var resultDto = new BirthdayDto(1, "Иван", "Иванов", new DateOnly(2000, 1, 1), null);
        _mockService.Setup(s => s.UpdateBirthdayAsync(id, dto)).ReturnsAsync(resultDto);

        // Act
        // Assert
        var result = await _controller.UpdateBirthday(id, dto);
        That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        That(okResult!.Value, Is.EqualTo(resultDto));
    }

    [Test]
    public async Task UpdateBirthday_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        const int id = 999;
        var dto = new UpdateBirthdayDto("Иван", "Иванов", new DateOnly(2000, 1, 1));
        _mockService.Setup(s => s.UpdateBirthdayAsync(id, dto)).ReturnsAsync((BirthdayDto)null);

        // Act
        var result = await _controller.UpdateBirthday(id, dto);
        
        // Assert
        That(result.Result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task DeleteBirthday_CallsServiceAndReturnsNoContent()
    {
        // Arrange
        const int id = 1;

        // Act
        var result = await _controller.DeleteBirthday(id);
        
        // Assert
        That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public async Task UploadPhoto_CallsServiceAndReturnsOk()
    {
        // Arrange
        const int id = 1;
        var fileMock = new Mock<IFormFile>();
        _mockService.Setup(s => s.UploadPhotoAsync(id, fileMock.Object)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UploadPhoto(id, fileMock.Object);
        
        // Assert
        That(result, Is.InstanceOf<OkResult>());
    }
}
