using System.ComponentModel.DataAnnotations;

namespace Birthdays.Api.Models.Entities;

public class Birthday
{
    public int Id { get; init; }
    [Required][StringLength(30)] public required string FirstName { get; init; }
    [Required][StringLength(30)] public required string LastName { get; init; }
    public DateTime BirthDay { get; init; }
    [StringLength(100)] public string? PhotoPath { get; init; }
}