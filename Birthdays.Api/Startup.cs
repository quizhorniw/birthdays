using Birthdays.Api.DbContexts;
using Birthdays.Api.Repositories;

namespace Birthdays.Api;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddNpgsql<BirthdaysDbContext>(configuration["ConnectionStrings:DefaultConnection"]);
        services.AddScoped<IBirthdaysRepository, BirthdaysRepository>();
        
        services.AddControllers();
    }
}