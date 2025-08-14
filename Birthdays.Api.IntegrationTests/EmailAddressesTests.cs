using Birthdays.Api.DbContexts;
using Birthdays.Api.Models.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using static NUnit.Framework.Assert;

namespace Birthdays.Api.IntegrationTests;

[TestFixture]
public class EmailAddressesTest
{
    private WebApplicationFactory<Startup> _factory;
    private BirthdaysDbContext _context;
    private HttpClient _client;

    [OneTimeSetUp]
    public void GlobalSetup()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
        
        _factory = new WebApplicationFactory<Startup>();
        _context = _factory.Services.CreateScope().ServiceProvider.GetRequiredService<BirthdaysDbContext>();
    }

    [SetUp]
    public void Setup()
    {
        _client = _factory.CreateClient();
    }

    [OneTimeTearDown]
    public void GlobalTearDown()
    {
        _factory.Dispose();   
        _context.Dispose();
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
    }

    [Test]
    public async Task InsertEmail_ResponseStatusOk()
    {
        // Arrange
        const string email = "test@test.com";
        
        // Act
        var response = await _client.PostAsync($"/api/emails?email={email}", null);
        
        // Assert
        response.EnsureSuccessStatusCode();
        That(await _context.EmailAddresses.FindAsync(email), Is.Not.Null);
    }
}