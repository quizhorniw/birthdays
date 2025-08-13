using Birthdays.Api.DbContexts;
using Birthdays.Api.Repositories;
using Birthdays.Api.Services;
using Hangfire;
using Hangfire.PostgreSql;

namespace Birthdays.Api;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddNpgsql<BirthdaysDbContext>(configuration["ConnectionStrings:BirthdaysDBConnection"]);
        services.AddScoped<IBirthdaysRepository, BirthdaysRepository>();
        services.AddScoped<IBirthdaysService, BirthdaysService>();
        
        services.AddControllers();

        services.AddHangfire(g => g
            .UsePostgreSqlStorage(opts => opts
                .UseNpgsqlConnection(configuration["ConnectionStrings:HangfireDBConnection"])));
        services.AddHangfireServer();

        services.AddScoped<IEmailAddressesRepository, EmailAddressesRepository>();
        services.AddScoped<IEmailAddressesService, EmailAddressesService>();
        services.AddScoped<IEmailSender, EmailSender>();
        services.AddScoped<EmailSenderJob>();
    }
}