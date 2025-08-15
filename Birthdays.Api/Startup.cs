using Birthdays.Api.DbContexts;
using Birthdays.Api.Repositories;
using Birthdays.Api.Services;
using Birthdays.Api.Services.Helpers;
using Birthdays.Api.Services.Wrappers;
using Hangfire;
using Hangfire.PostgreSql;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;

namespace Birthdays.Api;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins",
                builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
        });
        
        if (!Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")!.Equals("Testing",
                StringComparison.OrdinalIgnoreCase))
        {
            services.AddNpgsql<BirthdaysDbContext>(configuration["ConnectionStrings:BirthdaysDBConnection"]);
            
            services.AddHangfire(g => g
                .UsePostgreSqlStorage(opts => opts
                    .UseNpgsqlConnection(configuration["ConnectionStrings:HangfireDBConnection"])));
            services.AddHangfireServer();
        }
        else
        {
            services.AddDbContext<BirthdaysDbContext>(opts => opts.UseInMemoryDatabase("TestDb"));
        }
        
        services.AddScoped<IBirthdaysRepository, BirthdaysRepository>();
        services.AddScoped<IBirthdaysService, BirthdaysService>();
        
        services.AddControllers();

        services.AddScoped<IEmailAddressesRepository, EmailAddressesRepository>();
        services.AddScoped<IEmailAddressesService, EmailAddressesService>();
        services.AddTransient<SmtpClient>();
        services.AddTransient<ISmtpClientWrapper, SmtpClientWrapper>();
        services.AddScoped<IEmailSender, EmailSender>();
        services.AddScoped<IEmailHtmlParserHelper, EmailHtmlParserHelper>();
        services.AddScoped<EmailSenderJob>();
    }
}