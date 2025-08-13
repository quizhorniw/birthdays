using Birthdays.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Birthdays.Api.Controllers;

[ApiController]
[Route("/api/emails")]
public class EmailAddressesController(IEmailAddressesService emailAddressesService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> AddEmail(string email)
    {
        await emailAddressesService.InsertEmailAddressAsync(email);
        return Ok();
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteEmail(string email)
    {
        await emailAddressesService.DeleteEmailAddressAsync(email);
        return NoContent();
    }
}