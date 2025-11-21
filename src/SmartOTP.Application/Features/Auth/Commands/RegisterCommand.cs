using MediatR;
using SmartOTP.Application.DTOs;

namespace SmartOTP.Application.Features.Auth.Commands;

public class RegisterCommand : IRequest<AuthResponseDto>
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}
