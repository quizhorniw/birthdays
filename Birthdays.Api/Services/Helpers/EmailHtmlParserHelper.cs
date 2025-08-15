using Birthdays.Api.Extensions;

namespace Birthdays.Api.Services.Helpers;

public class EmailHtmlParserHelper(IConfiguration configuration, IBirthdaysService birthdaysService) : IEmailHtmlParserHelper
{
    public string GetParsedHtml()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var twoWeeksFromNow = today.AddDays(14);
        
        EmailHtmlParser.Parse(
            birthdaysService.GetBirthdaysAsync().Result
                .Where(b =>
                {
                    var birthdayThisYear = new DateOnly(today.Year, b.BirthDay.Month, b.BirthDay.Day);
                    var birthdayNextYear = new DateOnly(today.Year + 1, b.BirthDay.Month, b.BirthDay.Day);

                    return (birthdayThisYear >= today && birthdayThisYear <= twoWeeksFromNow) ||
                           (birthdayNextYear >= today && birthdayNextYear <= twoWeeksFromNow);
                })
                .Select(dto => dto.ToEmailString()),
            configuration["Html:BirthdaysEmailTemplate"]!,
            configuration["Html:BirthdaysEmail"]!);

        return File.ReadAllText(configuration["Html:BirthdaysEmail"]!);
    }
}

public interface IEmailHtmlParserHelper
{
    string GetParsedHtml();
}