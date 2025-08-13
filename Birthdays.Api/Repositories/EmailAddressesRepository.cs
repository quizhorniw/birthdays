using Birthdays.Api.DbContexts;
using Birthdays.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Birthdays.Api.Repositories;

public class EmailAddressesRepository(BirthdaysDbContext context) : IEmailAddressesRepository
{
    public async Task<IEnumerable<EmailAddress>> GetEmailAddressesAsync()
    {
        return await context.EmailAddresses.ToListAsync();
    }

    public async Task InsertEmailAddressAsync(string emailAddress)
    {
        await context.EmailAddresses.AddAsync(new EmailAddress { Value = emailAddress });
    }

    public async Task DeleteEmailAddressAsync(string emailAddress)
    {
        await context.EmailAddresses.Where(e => e.Value == emailAddress).ExecuteDeleteAsync();
    }

    public async Task SaveAsync()
    {
        await context.SaveChangesAsync();
    }
    
    private bool _disposed;

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                context.Dispose();
            }
        }
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}

public interface IEmailAddressesRepository : IDisposable
{
    Task<IEnumerable<EmailAddress>> GetEmailAddressesAsync();
    Task InsertEmailAddressAsync(string emailAddress);
    Task DeleteEmailAddressAsync(string emailAddress);
    Task SaveAsync();
}