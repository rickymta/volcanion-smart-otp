using Xunit;
using FluentAssertions;
using SmartOTP.Domain.Entities;

namespace SmartOTP.Domain.Tests.Entities;

public class UserTests
{
    [Fact]
    public void Create_ValidUser_ShouldSucceed()
    {
        // Arrange
        var email = "test@example.com";
        var passwordHash = "hashed_password";
        var firstName = "John";
        var lastName = "Doe";

        // Act
        var user = User.Create(email, passwordHash, firstName, lastName);

        // Assert
        user.Should().NotBeNull();
        user.Email.Should().Be(email.ToLowerInvariant());
        user.PasswordHash.Should().Be(passwordHash);
        user.FirstName.Should().Be(firstName);
        user.LastName.Should().Be(lastName);
        user.IsEmailVerified.Should().BeFalse();
        user.DomainEvents.Count.Should().Be(1);
    }

    [Theory]
    [InlineData("", "password")]
    [InlineData("email@test.com", "")]
    public void Create_InvalidParameters_ShouldThrowException(string email, string passwordHash)
    {
        // Act & Assert
        var action = () => User.Create(email, passwordHash);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void VerifyEmail_ShouldSetIsEmailVerifiedToTrue()
    {
        // Arrange
        var user = User.Create("test@example.com", "hashed_password");

        // Act
        user.VerifyEmail();

        // Assert
        user.IsEmailVerified.Should().BeTrue();
    }

    [Fact]
    public void SetRefreshToken_ShouldUpdateTokenAndExpiry()
    {
        // Arrange
        var user = User.Create("test@example.com", "hashed_password");
        var token = "refresh_token";
        var expiry = DateTime.UtcNow.AddDays(7);

        // Act
        user.SetRefreshToken(token, expiry);

        // Assert
        user.RefreshToken.Should().Be(token);
        user.RefreshTokenExpiryTime.Should().Be(expiry);
    }

    [Fact]
    public void IsRefreshTokenValid_ValidToken_ShouldReturnTrue()
    {
        // Arrange
        var user = User.Create("test@example.com", "hashed_password");
        user.SetRefreshToken("token", DateTime.UtcNow.AddDays(1));

        // Act
        var isValid = user.IsRefreshTokenValid();

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void IsRefreshTokenValid_ExpiredToken_ShouldReturnFalse()
    {
        // Arrange
        var user = User.Create("test@example.com", "hashed_password");
        user.SetRefreshToken("token", DateTime.UtcNow.AddDays(-1));

        // Act
        var isValid = user.IsRefreshTokenValid();

        // Assert
        isValid.Should().BeFalse();
    }
}
