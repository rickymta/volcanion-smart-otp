using MediatR;
using SmartOTP.Application.DTOs;

namespace SmartOTP.Application.Features.Auth.Commands;

public class RefreshTokenCommand : IRequest<AuthResponseDto>
{
    public string RefreshToken { get; set; } = null!;
}
