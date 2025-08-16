using System.ComponentModel.DataAnnotations;
using Birthdays.Api.Models.Entities;
using Birthdays.Api.Repositories;

namespace Birthdays.Api.Services;

public class EmailAddressesService(IEmailAddressesRepository emailAddressesRepository) : IEmailAddressesService
{
    public async Task<IEnumerable<EmailAddress>> GetEmailAddressesAsync()
    {
        return await emailAddressesRepository.GetEmailAddressesAsync();
    }

    public async Task InsertEmailAddressAsync(string emailAddress)
    {
        var emailAddressAttribute = new EmailAddressAttribute();
        if (emailAddressAttribute.IsValid(emailAddress))
        {
            var emailAddresses = await emailAddressesRepository.GetEmailAddressesAsync();
            if (emailAddresses.All(e => e.Value != emailAddress))
            {
                await emailAddressesRepository.InsertEmailAddressAsync(emailAddress);
                await emailAddressesRepository.SaveAsync();
            }
        }
    }

    public async Task DeleteEmailAddressAsync(string emailAddress)
    {
        await emailAddressesRepository.DeleteEmailAddressAsync(emailAddress);
    }
}

public interface IEmailAddressesService
{
    Task<IEnumerable<EmailAddress>> GetEmailAddressesAsync();
    Task InsertEmailAddressAsync(string emailAddress);
    Task DeleteEmailAddressAsync(string emailAddress);
}