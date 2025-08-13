using Birthdays.Api.Models.Dtos;
using Birthdays.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Birthdays.Api.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class BirthdaysController(IBirthdaysService birthdaysService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BirthdayDto>>> GetBirthdays()
    {
        var birthdays = await birthdaysService.GetBirthdaysAsync();
        return Ok(birthdays);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BirthdayDto>> GetBirthday(int id)
    {
        var birthday = await birthdaysService.GetBirthdayAsync(id);
        return birthday is not null ? Ok(birthday) : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<BirthdayDto>> InsertBirthday(CreateBirthdayDto createBirthdayDto)
    {
        var birthday = await birthdaysService.InsertBirthdayAsync(createBirthdayDto);
        return CreatedAtAction(nameof(GetBirthday), new { birthday.Id }, birthday);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<BirthdayDto>> UpdateBirthday(int id, UpdateBirthdayDto updateBirthdayDto)
    {
        var birthday = await birthdaysService.UpdateBirthdayAsync(id, updateBirthdayDto);
        return birthday is not null ? Ok(birthday) : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteBirthday(int id)
    {
        await birthdaysService.DeleteBirthdayAsync(id);
        return NoContent();
    }

    [HttpPost("uploadPic/{id:int}")]
    public async Task<ActionResult> UploadPhoto(int id, IFormFile file)
    {
        await birthdaysService.UploadPhotoAsync(id, file);
        return Ok();
    }
}