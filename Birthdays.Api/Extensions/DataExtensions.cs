using Birthdays.Api.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Birthdays.Api.Extensions;

public static class DataExtensions
{
    public static async void MigrateDb(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BirthdaysDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}