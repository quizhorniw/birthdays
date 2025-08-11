using Birthdays.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Birthdays.Api.DbContexts;

public class BirthdaysDbContext(DbContextOptions<BirthdaysDbContext> opts) : DbContext(opts)
{
    public DbSet<Birthday> Birthdays { get; set; }
}