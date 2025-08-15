using Birthdays.Api;
using Birthdays.Api.Extensions;
using Birthdays.Api.Services;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseCors("AllowAllOrigins");

app.MapControllers();

if (!Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")!.Equals("Testing",
        StringComparison.OrdinalIgnoreCase))
{
    app.UseHangfireDashboard();
    using var scope = app.Services.CreateScope();
    var emailSenderJob = scope.ServiceProvider.GetService<EmailSenderJob>();
    emailSenderJob?.ScheduleJob();
    
    app.MigrateDb();
}

app.Run();