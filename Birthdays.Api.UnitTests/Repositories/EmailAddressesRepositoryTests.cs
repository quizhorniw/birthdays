using Birthdays.Api.DbContexts;
using Birthdays.Api.Models.Entities;
using Birthdays.Api.Repositories;
using Microsoft.EntityFrameworkCore;
using static NUnit.Framework.Assert;

namespace Birthdays.Api.UnitTests.Repositories;

[TestFixture]
public class EmailAddressesRepositoryTests
{
    private BirthdaysDbContext _context;
    private EmailAddressesRepository _repository;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<BirthdaysDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new BirthdaysDbContext(options);
        _repository = new EmailAddressesRepository(_context);
    }

    [Test]
    public async Task GetEmailAddressesAsync_ReturnsAllEmails()
    {
        // Arrange
        await _repository.InsertEmailAddressAsync("test@example.com");
        await _repository.SaveAsync();

        // Act
        var result = await _repository.GetEmailAddressesAsync();

        // Assert
        var resulArray = result as EmailAddress[] ?? result.ToArray();
        Multiple(() =>
        {
            That(resulArray, Has.Length.EqualTo(1));
        });
        That(resulArray.First().Value, Is.EqualTo("test@example.com"));
    }

    [Test]
    public async Task InsertEmailAddressAsync_AddsNewEmail()
    {
        // Arrange & Act
        await _repository.InsertEmailAddressAsync("new@example.com");
        await _repository.SaveAsync();

        // Assert
        var email = await _context.EmailAddresses.FirstOrDefaultAsync(e => e.Value == "new@example.com");
        That(email, Is.Not.Null);
    }

    [Test]
    public async Task DeleteEmailAddressAsync_RemovesExistingEmail()
    {
        // Arrange
        await _repository.InsertEmailAddressAsync("delete@example.com");
        await _repository.SaveAsync();

        // Act
        _context.ChangeTracker.Clear();
        _context.EmailAddresses.Remove(new EmailAddress { Value = "delete@example.com" });
        await _context.SaveChangesAsync();

        // Assert
        var email = await _context.EmailAddresses.FirstOrDefaultAsync(e => e.Value == "delete@example.com");
        That(email, Is.Null);
    }

    [Test]
    public async Task SaveAsync_PersistsChanges()
    {
        // Arrange
        await _repository.InsertEmailAddressAsync("save@example.com");

        // Act
        await _repository.SaveAsync();

        // Assert
        var email = await _context.EmailAddresses.FirstOrDefaultAsync(e => e.Value == "save@example.com");
        That(email, Is.Not.Null);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        _repository.Dispose();
    }
}