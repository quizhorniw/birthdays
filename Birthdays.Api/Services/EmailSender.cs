using Birthdays.Api.Extensions;
using Birthdays.Api.Models.Dtos;
using Birthdays.Api.Models.Entities;
using Hangfire;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;

namespace Birthdays.Api.Services;

public class EmailSender(IConfiguration configuration, IBirthdaysService birthdaysService) : IEmailSender
{
    public async Task SendEmailsAsync(IEnumerable<EmailAddress> tos, string subject, string text)
    {
        var message = CreateMimeMessage(tos, subject, text);
        
        await ConnectSmtpAndSendMessage(message);
    }

    private MimeMessage CreateMimeMessage(IEnumerable<EmailAddress> tos, string subject, string text)
    {
        var username = configuration["Smtp:Username"];

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(username, username));
        message.Subject = subject;
        message.Body = new TextPart(TextFormat.Html)
        {
            Text = text
        };
        message.To.AddRange(tos.Select(t => new MailboxAddress(t.Value, t.Value)));

        return message;
    }

    private async Task ConnectSmtpAndSendMessage(MimeMessage message)
    {
        var smtpServer = configuration["Smtp:Host"];
        var port = int.Parse(configuration["Smtp:Port"]!);
        var username = configuration["Smtp:Username"];
        var password = configuration["Smtp:Password"];
        var useSsl = bool.Parse(configuration["Smtp:UseSSL"]!);

        using var client = new SmtpClient();
        await client.ConnectAsync(smtpServer, port, useSsl).ConfigureAwait(false);
        await client.AuthenticateAsync(username, password).ConfigureAwait(false);
        await client.SendAsync(message).ConfigureAwait(false);
        await client.DisconnectAsync(true).ConfigureAwait(false);
    }

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

public class EmailSenderJob(
    IEmailSender emailSender,
    IConfiguration configuration,
    IEmailAddressesService emailAddressesService)
{
    public void ScheduleJob()
    {
        RecurringJob.AddOrUpdate(
            "email-job",
            () => ExecuteEmailJob(),
            // configuration["Job:Cron"]);
            "*/5 * * * *");
    }

    public async Task ExecuteEmailJob()
    {
        var emailAddresses = await emailAddressesService.GetEmailAddressesAsync();
        await emailSender.SendEmailsAsync(emailAddresses, configuration["Smtp:Subject"]!, emailSender.GetParsedHtml());
    }
}

public interface IEmailSender
{
    Task SendEmailsAsync(IEnumerable<EmailAddress> tos, string subject, string message);
    string GetParsedHtml();
}