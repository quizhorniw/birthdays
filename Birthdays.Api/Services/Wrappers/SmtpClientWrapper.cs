using MailKit.Net.Smtp;
using MimeKit;

namespace Birthdays.Api.Services.Wrappers;

public class SmtpClientWrapper(SmtpClient client) : ISmtpClientWrapper
{
    public async Task ConnectAsync(string? host, int port, bool useSsl)
    {
        await client.ConnectAsync(host, port, useSsl);
    }

    public async Task AuthenticateAsync(string? username, string? password)
    {
        await client.AuthenticateAsync(username, password);
    }

    public async Task SendAsync(MimeMessage message)
    {
        await client.SendAsync(message);
    }

    public async Task DisconnectAsync(bool quit)
    {
        await client.DisconnectAsync(quit);
    }
}


public interface ISmtpClientWrapper
{
    Task ConnectAsync(string? host, int port, bool useSsl);
    Task AuthenticateAsync(string? username, string? password);
    Task SendAsync(MimeMessage message);
    Task DisconnectAsync(bool quit);
}