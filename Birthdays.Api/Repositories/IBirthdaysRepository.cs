using Birthdays.Api.Models.Entities;

namespace Birthdays.Api.Repositories;

public interface IBirthdaysRepository : IDisposable
{
    Task<IEnumerable<Birthday>> GetBirthdaysAsync();
    Task<Birthday> GetBirthdayAsync(int id);
    Task InsertBirthdayAsync(Birthday birthday);
    Task<bool> UpdateBirthdayAsync(Birthday birthday, int id);
    Task DeleteBirthdayAsync(int id);
    Task SaveAsync();
}