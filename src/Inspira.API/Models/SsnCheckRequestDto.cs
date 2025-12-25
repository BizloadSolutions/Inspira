namespace Inspira.API.Models;

public sealed class SsnCheckRequestDto
{
    public string SSN { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
