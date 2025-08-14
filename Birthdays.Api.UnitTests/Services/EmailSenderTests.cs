using Birthdays.Api.Models.Entities;
using Birthdays.Api.Services;
using Birthdays.Api.Services.Wrappers;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Moq;

namespace Birthdays.Api.UnitTests.Services;

[TestFixture]
public class EmailSenderTests
{
    private Mock<IConfiguration> _configurationMock;
    private Mock<ISmtpClientWrapper> _smtpClientMock;
    private EmailSender _emailSender;

    [SetUp]
    public void Setup()
    {
        _configurationMock = new Mock<IConfiguration>();
        _smtpClientMock = new Mock<ISmtpClientWrapper>();

        _configurationMock.SetupGet(x => x["Smtp:Username"]).Returns("test@example.com");
        _configurationMock.SetupGet(x => x["Smtp:Host"]).Returns("smtp.example.com");
        _configurationMock.SetupGet(x => x["Smtp:Port"]).Returns("587");
        _configurationMock.SetupGet(x => x["Smtp:Password"]).Returns("password");
        _configurationMock.SetupGet(x => x["Smtp:UseSSL"]).Returns("true");

        _emailSender = new EmailSender(_configurationMock.Object, _smtpClientMock.Object);
    }

    [Test]
    public async Task SendEmailsAsync_ShouldCallSmtpClientMethods_WhenValidInput()
    {
        // Arrange
        var tos = new List<EmailAddress>
        {
            new() { Value = "user1@example.com" },
            new() { Value = "user2@example.com" }
        };
        const string subject = "Test Subject";
        const string text = "<p>Hello!</p>";

        // Act
        await _emailSender.SendEmailsAsync(tos, subject, text);

        // Assert
        _smtpClientMock.Verify(client => client
            .ConnectAsync("smtp.example.com", 587, true), Times.Once);
        _smtpClientMock.Verify(client => client
            .AuthenticateAsync("test@example.com", "password"), Times.Once);
        _smtpClientMock.Verify(client => client
            .SendAsync(It.IsAny<MimeMessage>()), Times.Once);
        _smtpClientMock.Verify(client => client
            .DisconnectAsync(true), Times.Once);
    }

    [Test]
    public void SendEmailsAsync_ShouldThrowException_WhenSmtpConnectFails()
    {
        // Arrange
        _smtpClientMock.Setup(client => client
            .ConnectAsync("smtp.example.com", 587, true))
            .ThrowsAsync(new Exception("Connection failed"));

        var tos = new List<EmailAddress> { new() { Value = "user1@example.com" } };
        const string subject = "Subject";
        const string text = "Body";

        // Act
        // Assert
        Assert.ThrowsAsync<Exception>(() => _emailSender.SendEmailsAsync(tos, subject, text));
    }

    [Test]
    public void SendEmailsAsync_ShouldThrowException_WhenSmtpAuthFails()
    {
        // Arrange
        _smtpClientMock.Setup(client => client
            .ConnectAsync("smtp.example.com", 587, true))
            .Returns(Task.CompletedTask);
        _smtpClientMock.Setup(client => client
            .AuthenticateAsync("test@example.com", "password"))
            .ThrowsAsync(new Exception("Authentication failed"));

        var tos = new List<EmailAddress> { new() { Value = "user1@example.com" } };
        const string subject = "Subject";
        const string text = "Body";

        // Act
        // Assert
        Assert.ThrowsAsync<Exception>(() => _emailSender.SendEmailsAsync(tos, subject, text));
    }

    [Test]
    public void SendEmailsAsync_ShouldThrowException_WhenSmtpSendFails()
    {
        // Arrange
        _smtpClientMock.Setup(client => client
            .ConnectAsync("smtp.example.com", 587, true))
            .Returns(Task.CompletedTask);
        _smtpClientMock.Setup(client => client
            .AuthenticateAsync("test@example.com", "password"))
            .Returns(Task.CompletedTask);
        _smtpClientMock.Setup(client => client
            .SendAsync(It.IsAny<MimeMessage>()))
            .ThrowsAsync(new Exception("Send failed"));

        var tos = new List<EmailAddress> { new() { Value = "user1@example.com" } };
        const string subject = "Subject";
        const string text = "Body";

        // Act
        // Assert
        Assert.ThrowsAsync<Exception>(() => _emailSender.SendEmailsAsync(tos, subject, text));
    }

    [Test]
    public void SendEmailsAsync_ShouldThrowException_WhenSubjectIsNull()
    {
        // Arrange
        var tos = new List<EmailAddress> { new() { Value = "user1@example.com" } };
        string? subject = null;
        const string text = "Body";

        // Act
        // Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _emailSender.SendEmailsAsync(tos, subject, text));
    }

    [Test]
    public void SendEmailsAsync_ShouldThrowException_WhenTextIsNull()
    {
        // Arrange
        var tos = new List<EmailAddress> { new() { Value = "user1@example.com" } };
        const string subject = "Subject";
        string? text = null;

        // Act
        // Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _emailSender.SendEmailsAsync(tos, subject, text));
    }
}