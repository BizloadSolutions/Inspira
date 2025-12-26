using System.ComponentModel.DataAnnotations;

namespace Inspira.API.Models;

public sealed class SsnCheckRequestDto
{
    [RegularExpression(@"^[\d•]+$", ErrorMessage = "SSN can only contain digits and dots.")]
    public string SSN { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
