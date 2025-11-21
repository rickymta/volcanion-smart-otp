using MediatR;
using SmartOTP.Application.Common.Interfaces;
using SmartOTP.Application.DTOs;
using SmartOTP.Domain.Entities;
using SmartOTP.Domain.Enums;

namespace SmartOTP.Application.Features.Auth.Commands;

public class RegisterCommandHandler(
    IRepository<User> userRepository,
    IPasswordHasher passwordHasher,
    IJwtService jwtService,
    IAuditService auditService,
    IUnitOfWork unitOfWork) : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // Check if user already exists
        var existingUser = await userRepository.FirstOrDefaultAsync(u => u.Email.Equals(request.Email), cancellationToken);
        if (existingUser != null)
        {
            await auditService.LogAsync(AuditLog.CreateFailure(null, AuditActionType.UserRegistered, "Email already exists", details: request.Email), cancellationToken);
            throw new InvalidOperationException("User with this email already exists");
        }

        // Create new user
        var passwordHash = passwordHasher.HashPassword(request.Password);
        var user = User.Create(request.Email, passwordHash, request.FirstName, request.LastName);

        // Generate tokens
        var accessToken = jwtService.GenerateAccessToken(user.Id, user.Email);
        var refreshToken = jwtService.GenerateRefreshToken();
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        user.SetRefreshToken(refreshToken, refreshTokenExpiry);

        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Log success
        await auditService.LogAsync(AuditLog.CreateSuccess(user.Id, AuditActionType.UserRegistered), cancellationToken);

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
