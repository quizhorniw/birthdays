using Birthdays.Api.Models.Dtos;
using Birthdays.Api.Models.Entities;

namespace Birthdays.Api.Extensions;

public static class BirthdaysExtensions
{
    public static BirthdayDto ToDto(this Birthday birthday)
    {
        return new BirthdayDto(
            birthday.Id,
            birthday.FirstName,
            birthday.LastName,
            birthday.BirthDay
        );
    }

    public static Birthday ToEntity(this CreateBirthdayDto createBirthdayDto)
    {
        return new Birthday
        {
            FirstName = createBirthdayDto.FirstName,
            LastName = createBirthdayDto.LastName,
            BirthDay = createBirthdayDto.BirthDay
        };
    }
    
    public static Birthday ToEntity(this UpdateBirthdayDto updateBirthdayDto)
    {
        return new Birthday
        {
            FirstName = updateBirthdayDto.FirstName,
            LastName = updateBirthdayDto.LastName,
            BirthDay = updateBirthdayDto.BirthDay
        };
    }
}