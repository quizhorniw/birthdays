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
                    var birthday = new DateOnly(today.Year, b.BirthDay.Month, b.BirthDay.Day);
                    return birthday >= today && birthday <= twoWeeksFromNow;
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