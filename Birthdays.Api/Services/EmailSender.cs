using Birthdays.Api.Models.Entities;
using Birthdays.Api.Services.Helpers;
using Birthdays.Api.Services.Wrappers;
using Hangfire;
using MimeKit;
using MimeKit.Text;

namespace Birthdays.Api.Services;

public class EmailSender(IConfiguration configuration, ISmtpClientWrapper client) : IEmailSender
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

        await client.ConnectAsync(smtpServer, port, useSsl).ConfigureAwait(false);
        await client.AuthenticateAsync(username, password).ConfigureAwait(false);
        await client.SendAsync(message).ConfigureAwait(false);
        await client.DisconnectAsync(true).ConfigureAwait(false);
    }
}

public class EmailSenderJob(
    IEmailSender emailSender,
    IConfiguration configuration,
    IEmailAddressesService emailAddressesService,
    IEmailHtmlParserHelper htmlParserHelper)
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
        await emailSender.SendEmailsAsync(emailAddresses, configuration["Smtp:Subject"]!, htmlParserHelper.GetParsedHtml());
    }
}

public interface IEmailSender
{
    Task SendEmailsAsync(IEnumerable<EmailAddress> tos, string subject, string message);
}