namespace Birthdays.Api.Models.Dtos;

public record BirthdayDto(int Id, string FirstName, string LastName, DateOnly BirthDay);