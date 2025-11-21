using MediatR;
using SmartOTP.Application.Common.Interfaces;
using SmartOTP.Application.DTOs;
using SmartOTP.Domain.Entities;
using SmartOTP.Domain.Enums;

namespace SmartOTP.Application.Features.Auth.Commands;

public class RefreshTokenCommandHandler(
    IRepository<User> userRepository,
    IJwtService jwtService,
    IAuditService auditService,
    IUnitOfWork unitOfWork) : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken, cancellationToken);
        if (user == null || !user.IsRefreshTokenValid())
        {
            await auditService.LogAsync(AuditLog.CreateFailure(null, AuditActionType.RefreshTokenUsed, "Invalid or expired refresh token"), cancellationToken);
            throw new UnauthorizedAccessException("Invalid or expired refresh token");
        }

        // Generate new tokens
        var accessToken = jwtService.GenerateAccessToken(user.Id, user.Email);
        var refreshToken = jwtService.GenerateRefreshToken();
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        user.SetRefreshToken(refreshToken, refreshTokenExpiry);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.LogAsync(AuditLog.CreateSuccess(user.Id, AuditActionType.RefreshTokenUsed), cancellationToken);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiresAt = DateTime.UtcNow.AddHours(1),
            RefreshTokenExpiresAt = refreshTokenExpiry,
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsEmailVerified = user.IsEmailVerified,
                LastLoginAt = user.LastLoginAt,
                CreatedAt = user.CreatedAt
            }
        };
    }
}
