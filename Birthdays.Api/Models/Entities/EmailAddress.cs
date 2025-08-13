using System.ComponentModel.DataAnnotations;

namespace Birthdays.Api.Models.Entities;

public class EmailAddress
{
    [Key][Required][StringLength(80)][EmailAddress]
    public required string Value { get; init; }
}