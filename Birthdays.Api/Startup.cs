using Birthdays.Api.DbContexts;
using Birthdays.Api.Repositories;
using Birthdays.Api.Services;

namespace Birthdays.Api;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddNpgsql<BirthdaysDbContext>(configuration["ConnectionStrings:DefaultConnection"]);
        services.AddScoped<IBirthdaysRepository, BirthdaysRepository>();
        services.AddScoped<IBirthdaysService, BirthdaysService>();
        
        services.AddControllers();
    }
}