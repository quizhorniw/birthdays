using Birthdays.Api.Extensions;
using Birthdays.Api.Models.Dtos;
using Birthdays.Api.Repositories;

namespace Birthdays.Api.Services;

public class BirthdaysService(IBirthdaysRepository birthdaysRepository) : IBirthdaysService
{
    public async Task<IEnumerable<BirthdayDto>> GetBirthdaysAsync()
    {
        var birthdays = await birthdaysRepository.GetBirthdaysAsync();
        return birthdays.Select(b => b.ToDto());
    }

    public async Task<BirthdayDto?> GetBirthdayAsync(int id)
    {
        var birthday = await birthdaysRepository.GetBirthdayAsync(id);
        return birthday?.ToDto();
    }

    public async Task<BirthdayDto> InsertBirthdayAsync(CreateBirthdayDto createBirthdayDto)
    {
        var birthday = createBirthdayDto.ToEntity();
        await birthdaysRepository.InsertBirthdayAsync(birthday);
        await birthdaysRepository.SaveAsync();
        return birthday.ToDto();
    }

    public async Task<BirthdayDto?> UpdateBirthdayAsync(int id, UpdateBirthdayDto updateBirthdayDto)
    {
        var birthday = updateBirthdayDto.ToEntity();
        var updated = await birthdaysRepository.UpdateBirthdayAsync(id, birthday);
        await birthdaysRepository.SaveAsync();
        return updated ? birthday.ToDto() : null;
    }

    public async Task DeleteBirthdayAsync(int id)
    {
        await birthdaysRepository.DeleteBirthdayAsync(id);
    }

    public async Task UploadPhotoAsync(int id, IFormFile? file)
    {
        var birthday = await birthdaysRepository.GetBirthdayAsync(id);
        if (birthday is null)
        {
            return;
        }
        
        if (file is null || file.Length == 0)
        {
            return;
        }

        var uploads = GetPhotoDirectory();

        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(uploads, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        birthday.PhotoPath = filePath;
        await birthdaysRepository.UpdateBirthdayAsync(id, birthday);
    }

    private static string GetPhotoDirectory()
    {
        var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
        
        if (!Directory.Exists(uploads))
        {
            Directory.CreateDirectory(uploads);
        }

        return uploads;
    }
}

public interface IBirthdaysService
{
    Task<IEnumerable<BirthdayDto>> GetBirthdaysAsync();
    Task<BirthdayDto?> GetBirthdayAsync(int id);
    Task<BirthdayDto> InsertBirthdayAsync(CreateBirthdayDto createBirthdayDto);
    Task<BirthdayDto?> UpdateBirthdayAsync(int id, UpdateBirthdayDto updateBirthdayDto);
    Task DeleteBirthdayAsync(int id);
    Task UploadPhotoAsync(int id, IFormFile file);
}