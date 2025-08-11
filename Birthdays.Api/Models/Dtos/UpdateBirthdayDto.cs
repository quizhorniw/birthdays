namespace Birthdays.Api.Models.Dtos;

public record UpdateBirthdayDto(string FirstName, string LastName, DateOnly BirthDay);