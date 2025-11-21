using MediatR;
using SmartOTP.Application.DTOs;

namespace SmartOTP.Application.Features.Auth.Commands;

public class LoginCommand : IRequest<AuthResponseDto>
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}
