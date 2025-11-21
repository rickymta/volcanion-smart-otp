namespace SmartOTP.Application.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool IsEmailVerified { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
