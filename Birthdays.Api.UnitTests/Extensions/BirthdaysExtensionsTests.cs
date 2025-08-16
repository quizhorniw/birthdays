using Birthdays.Api.Extensions;
using Birthdays.Api.Models.Dtos;
using Birthdays.Api.Models.Entities;
using static NUnit.Framework.Assert;

namespace Birthdays.Api.UnitTests.Extensions;

[TestFixture]
public class BirthdaysExtensionsTests
{
    [Test]
    public void ToDto_ShouldMapBirthdayEntityToBirthdayDto()
    {
        // Arrange
        var birthday = new Birthday
        {
            Id = 1,
            FirstName = "Иван",
            LastName = "Иванов",
            BirthDay = new DateOnly(1990, 5, 15),
            PhotoFileName = "ivan.jpg"
        };

        // Act
        var dto = birthday.ToDto();
        
        // Assert
        That(dto.Id, Is.EqualTo(birthday.Id));
        That(dto.FirstName, Is.EqualTo(birthday.FirstName));
        That(dto.LastName, Is.EqualTo(birthday.LastName));
        That(dto.BirthDay, Is.EqualTo(birthday.BirthDay));
        That(dto.PhotoFileName, Is.EqualTo(birthday.PhotoFileName));
    }

    [Test]
    public void ToEntity_FromCreateBirthdayDto_ShouldMapCorrectly()
    {
        // Arrange
        var createDto = new CreateBirthdayDto("Алексей", "Смирнов", new DateOnly(1999, 1, 1));

        // Act
        var entity = createDto.ToEntity();
        
        // Assert
        That(entity.FirstName, Is.EqualTo(createDto.FirstName));
        That(entity.LastName, Is.EqualTo(createDto.LastName));
        That(entity.BirthDay, Is.EqualTo(createDto.BirthDay));
        That(entity.Id, Is.Zero);
        That(entity.PhotoFileName, Is.Null);
    }

    [Test]
    public void ToEntity_FromUpdateBirthdayDto_ShouldMapCorrectly()
    {
        // Arrange
        var updateDto = new UpdateBirthdayDto("Алексей", "Смирнов", new DateOnly(1999, 1, 1));

        // Act
        var entity = updateDto.ToEntity();
        
        // Assert
        That(entity.FirstName, Is.EqualTo(updateDto.FirstName));
        That(entity.LastName, Is.EqualTo(updateDto.LastName));
        That(entity.BirthDay, Is.EqualTo(updateDto.BirthDay));
        That(entity.Id, Is.Zero);
        That(entity.PhotoFileName, Is.Null);
    }

    [Test]
    public void ToEmailString_ShouldFormatDateCorrectly()
    {
        // Arrange
        var dto = new BirthdayDto(1, "Дмитрий", "Фёдоров", new DateOnly(2000, 6, 14), null);

        // Act
        var result = dto.ToEmailString();
        
        // Assert
        That(result, Is.EqualTo("Дмитрий Фёдоров - 14 июня"));
    }
    
    [Test]
    public void ToEmailString_ShouldUseRussianCulture()
    {
        // Arrange
        var dto = new BirthdayDto(1, "Дмитрий", "Фёдоров", new DateOnly(2000, 6, 14), null);

        // Act
        var result = dto.ToEmailString();
        
        // Assert
        That(result, Is.EqualTo("Дмитрий Фёдоров - 14 июня"));
    }
}
