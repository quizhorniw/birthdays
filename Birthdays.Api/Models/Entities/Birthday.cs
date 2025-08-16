using System.ComponentModel.DataAnnotations;

namespace Birthdays.Api.Models.Entities;

public class Birthday
{
    public int Id { get; set; }
    [Required][StringLength(30)] public required string FirstName { get; set; }
    [Required][StringLength(30)] public required string LastName { get; set; }
    public DateOnly BirthDay { get; set; }
    [StringLength(50)] public string? PhotoFileName { get; set; }
}