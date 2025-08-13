using Birthdays.Api.DbContexts;
using Birthdays.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Birthdays.Api.Repositories;

public class BirthdaysRepository(BirthdaysDbContext context) : IBirthdaysRepository
{
    public async Task<IEnumerable<Birthday>> GetBirthdaysAsync()
    {
        return await context.Birthdays.ToListAsync();
    }

    public async Task<Birthday?> GetBirthdayAsync(int id)
    {
        return await context.Birthdays.FindAsync(id);
    }

    public async Task InsertBirthdayAsync(Birthday birthday)
    {
        await context.Birthdays.AddAsync(birthday);
    }

    public async Task<bool> UpdateBirthdayAsync(int id, Birthday birthday)
    {
        var existingBirthday = await context.Birthdays.FindAsync(id);
        
        if (existingBirthday is null) return false;
        
        context.Entry(existingBirthday).CurrentValues.SetValues(birthday);

        return true;
    }

    public async Task DeleteBirthdayAsync(int id)
    {
        await context.Birthdays.Where(b => b.Id == id).ExecuteDeleteAsync();
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

public interface IBirthdaysRepository : IDisposable
{
    Task<IEnumerable<Birthday>> GetBirthdaysAsync();
    Task<Birthday?> GetBirthdayAsync(int id);
    Task InsertBirthdayAsync(Birthday birthday);
    Task<bool> UpdateBirthdayAsync(int id, Birthday birthday);
    Task DeleteBirthdayAsync(int id);
    Task SaveAsync();
}