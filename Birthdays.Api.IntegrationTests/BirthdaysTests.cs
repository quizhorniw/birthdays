using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Birthdays.Api.DbContexts;
using Birthdays.Api.Models.Dtos;
using Birthdays.Api.Models.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using static NUnit.Framework.Assert;

namespace Birthdays.Api.IntegrationTests;
    
[TestFixture]
public class BirthdaysTests
{
    private WebApplicationFactory<Startup> _factory;
    private BirthdaysDbContext _context;
    private HttpClient _client;
    private Birthday _testBirthday;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    [OneTimeSetUp]
    public async Task GlobalSetup()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
        
        _factory = new WebApplicationFactory<Startup>();
        _context = _factory.Services.CreateScope().ServiceProvider.GetRequiredService<BirthdaysDbContext>();

        _testBirthday = new Birthday
        {
            FirstName = "Иван",
            LastName = "Иванов",
            BirthDay = new DateOnly(2000, 1, 1)
        };
        await _context.Birthdays.AddAsync(_testBirthday);
        await _context.SaveChangesAsync();
    }
    
    [SetUp]
    public void SetUp()
    {
        _client = _factory.CreateClient();
    }

    [Test]
    public async Task GetBirthdays_ReturnsAllBirthdays()
    {
        // Act
        var response = await _client.GetAsync("/api/birthdays");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var birthdays = JsonSerializer.Deserialize<List<BirthdayDto>>(content, _jsonOptions);
        That(birthdays, Is.Not.Null);
        That(birthdays, Is.Not.Empty);
        var firstBirthday = birthdays!.First();
        That(firstBirthday.Id, Is.EqualTo(_testBirthday.Id));
        That(firstBirthday.FirstName, Is.EqualTo(_testBirthday.FirstName));
        That(firstBirthday.LastName, Is.EqualTo(_testBirthday.LastName));
        That(firstBirthday.BirthDay, Is.EqualTo(_testBirthday.BirthDay));
    }

    [Test]
    public async Task GetBirthdayById_ExistingId_ReturnsBirthday()
    {
        // Arrange
        const int id = 1;

        // Act
        var response = await _client.GetAsync($"/api/birthdays/{id}");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var birthday = JsonSerializer.Deserialize<BirthdayDto>(content, _jsonOptions);
        That(birthday, Is.Not.Null);
        That(birthday!.Id, Is.EqualTo(_testBirthday.Id));
        That(birthday.FirstName, Is.EqualTo(_testBirthday.FirstName));
        That(birthday.LastName, Is.EqualTo(_testBirthday.LastName));
        That(birthday.BirthDay, Is.EqualTo(_testBirthday.BirthDay));
    }

    [Test]
    public async Task GetBirthdayById_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        const int id = 999;

        // Act
        // Assert
        var response = await _client.GetAsync($"/api/birthdays/{id}");
        That((int)response.StatusCode, Is.EqualTo(404));
    }

    [Test]
    public async Task InsertBirthday_CreatesNewBirthday()
    {
        // Arrange
        var dto = new CreateBirthdayDto("Андрей", "Иванов", new DateOnly(2000, 1, 1));

        var json = JsonSerializer.Serialize(dto, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/birthdays", content);

        // Assert
        response.EnsureSuccessStatusCode();
        var createdDto = await response.Content.ReadFromJsonAsync<BirthdayDto>();
        That(createdDto, Is.Not.Null);
        That(createdDto!.Id, Is.Not.EqualTo(0));
    }
    
    [Test]
    public async Task InsertEmptyBirthday_DoesNotCreateNewBirthday()
    {
        // Arrange
        CreateBirthdayDto? dto = null;

        var json = JsonSerializer.Serialize(dto, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/birthdays", content);

        // Assert
        That((int)response.StatusCode, Is.EqualTo(400));
        var createdDto = await response.Content.ReadFromJsonAsync<BirthdayDto>();
        That(createdDto, Is.Not.Null);
        That(createdDto!.Id, Is.EqualTo(0));
    }
    
    [Test]
    public async Task UploadPhoto_ValidFile_ReturnsOk()
    {
        // Arrange
        const int id = 1;
        const string fileContent = "Тестовый файл.";
        var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));

        using var multipartContent = new MultipartFormDataContent();
        multipartContent.Add(new StreamContent(fileStream), "file", "test.txt");

        // Act
        var response = await _client.PostAsync($"/api/birthdays/uploadPic/{id}", multipartContent);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Test]
    public async Task UploadPhoto_NullFile_ReturnsBadRequest()
    {
        // Act
        // Assert
        var response = await _client.PostAsync("/api/birthdays/uploadPic/1", null);
        That((int)response.StatusCode, Is.EqualTo(400));
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
}