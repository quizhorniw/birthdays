namespace Birthdays.Api.Models.Dtos;

public record CreateBirthdayDto(string FirstName, string LastName, DateOnly BirthDay);