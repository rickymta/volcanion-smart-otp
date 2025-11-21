using MediatR;
using SmartOTP.Application.Common.Interfaces;
using SmartOTP.Application.DTOs;
using SmartOTP.Domain.Entities;
using SmartOTP.Domain.Enums;

namespace SmartOTP.Application.Features.Auth.Commands;

public class LoginCommandHandler(
    IRepository<User> userRepository,
    IPasswordHasher passwordHasher,
    IJwtService jwtService,
    IAuditService auditService,
    IUnitOfWork unitOfWork) : IRequestHandler<LoginCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.FirstOrDefaultAsync(u => u.Email.Equals(request.Email), cancellationToken);
        if (user == null || !passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            await auditService.LogAsync(AuditLog.CreateFailure(null, AuditActionType.UserLoggedIn, "Invalid credentials", details: request.Email), cancellationToken);
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Generate tokens
        var accessToken = jwtService.GenerateAccessToken(user.Id, user.Email);
        var refreshToken = jwtService.GenerateRefreshToken();
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        user.SetRefreshToken(refreshToken, refreshTokenExpiry);
        user.RecordLogin();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Log success
        await auditService.LogAsync(AuditLog.CreateSuccess(user.Id, AuditActionType.UserLoggedIn), cancellationToken);

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
