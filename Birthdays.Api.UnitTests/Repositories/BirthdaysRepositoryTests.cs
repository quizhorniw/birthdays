using Birthdays.Api.DbContexts;
using Birthdays.Api.Models.Entities;
using Birthdays.Api.Repositories;
using Microsoft.EntityFrameworkCore;
using static NUnit.Framework.Assert;

namespace Birthdays.Api.UnitTests.Repositories;

[TestFixture]
public class BirthdaysRepositoryTests
{
    private DbContextOptions<BirthdaysDbContext> _options;
    private BirthdaysDbContext _context;
    private BirthdaysRepository _repository;

    [SetUp]
    public void Setup()
    {
        _options = new DbContextOptionsBuilder<BirthdaysDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new BirthdaysDbContext(_options);
        _repository = new BirthdaysRepository(_context);
    }

    [Test]
    public async Task GetBirthdaysAsync_ReturnsAllBirthdays()
    {
        // Arrange
        var birthdays = new List<Birthday>
        {
            new()
            {
                Id = 1, FirstName = "Иван", LastName = "Иванов", BirthDay = new DateOnly(2000, 1, 1)
            },
            new()
            {
                Id = 2, FirstName = "Сергей", LastName = "Сергеев", BirthDay = new DateOnly(2000, 2, 2)
            }
        };

        birthdays.ForEach(b => _ = _repository.InsertBirthdayAsync(b));
        await _repository.SaveAsync();

        // Act
        var result = await _repository.GetBirthdaysAsync();

        // Assert
        var resultArray = result as Birthday[] ?? result.ToArray();
        That(resultArray, Is.Not.Null);
        That(resultArray, Has.Length.EqualTo(2));
    }

    [Test]
    public async Task GetBirthdayAsync_ReturnsBirthday_WhenExists()
    {
        // Arrange
        var birthday = new Birthday
        {
            Id = 1,  FirstName = "Иван", LastName = "Иванов", BirthDay = new DateOnly(2000, 1, 1)
        };
        await _repository.InsertBirthdayAsync(birthday);
        await _repository.SaveAsync();

        // Act
        var result = await _repository.GetBirthdayAsync(1);

        // Assert
        That(result, Is.Not.Null);
        That(result!.FirstName, Is.EqualTo("Иван"));
    }

    [Test]
    public async Task GetBirthdayAsync_ReturnsNull_WhenDoesNotExist()
    {
        // Act
        var result = await _repository.GetBirthdayAsync(999);

        // Assert
        That(result, Is.Null);
    }

    [Test]
    public async Task InsertBirthdayAsync_AddsBirthday()
    {
        // Arrange
        var birthday = new Birthday
        {
            Id = 3, FirstName = "Евгений", LastName = "Петров", BirthDay = new DateOnly(2000, 3, 3)
        };

        // Act
        await _repository.InsertBirthdayAsync(birthday);
        await _repository.SaveAsync();

        // Assert
        var result = await _context.Birthdays.FindAsync(3);
        That(result, Is.Not.Null);
        That(result!.FirstName, Is.EqualTo("Евгений"));
    }

    [Test]
    public async Task UpdateBirthdayAsync_ReturnsTrue_WhenBirthdayExists()
    {
        // Arrange
        var existingBirthday = new Birthday
        {
            Id = 4, FirstName = "Денис", LastName = "Немцов", BirthDay = new DateOnly(2000, 4, 4)
        };
        await _repository.InsertBirthdayAsync(existingBirthday);
        await _repository.SaveAsync();

        var updatedBirthday = new Birthday
        {
            Id = 4, FirstName = "Наталья", LastName = "Иванова", BirthDay = new DateOnly(2001, 4, 4)
        };

        // Act
        var result = await _repository.UpdateBirthdayAsync(4, updatedBirthday);

        // Assert
        That(result, Is.True);
        var dbBirthday = await _repository.GetBirthdayAsync(4);
        That(dbBirthday, Is.Not.Null);
        Multiple(() =>
        {
            That(dbBirthday!.FirstName, Is.EqualTo("Наталья"));
            That(dbBirthday.BirthDay, Is.EqualTo(new DateOnly(2001, 4, 4)));
        });
    }

    [Test]
    public async Task UpdateBirthdayAsync_ReturnsFalse_WhenBirthdayDoesNotExist()
    {
        // Arrange
        var birthday = new Birthday
        {
            Id = 999, FirstName = "Григорий", LastName = "Захаров", BirthDay = new DateOnly(2000, 5, 5)
        };

        // Act
        var result = await _repository.UpdateBirthdayAsync(999, birthday);

        // Assert
        That(result, Is.False);
    }

    [Test]
    public async Task DeleteBirthdayAsync_RemovesBirthday()
    {
        // Arrange
        var birthday = new Birthday
        {
            Id = 5, FirstName = "Любовь", LastName = "Петрова", BirthDay = new DateOnly(2000, 6, 6)
        };
        await _repository.InsertBirthdayAsync(birthday);
        await _repository.SaveAsync();

        // Act
        _context.Birthdays.Remove(birthday);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _context.Birthdays.FindAsync(5);
        That(result, Is.Null);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        _repository.Dispose();
    }
}