using Xunit;
using FluentAssertions;
using SmartOTP.Domain.Entities;
using SmartOTP.Domain.Enums;
using SmartOTP.Domain.ValueObjects;

namespace SmartOTP.Domain.Tests.Entities;

public class OtpAccountTests
{
    [Fact]
    public void CreateTOTP_ValidParameters_ShouldSucceed()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var issuer = "GitHub";
        var accountName = "user@example.com";
        var secret = SecretKey.Create("encrypted_secret");

        // Act
        var account = OtpAccount.CreateTOTP(userId, issuer, accountName, secret);

        // Assert
        account.Should().NotBeNull();
        account.UserId.Should().Be(userId);
        account.Issuer.Should().Be(issuer);
        account.AccountName.Should().Be(accountName);
        account.Type.Should().Be(OtpType.TOTP);
        account.Digits.Should().Be(6);
        account.Period.Should().Be(30);
        account.DomainEvents.Count.Should().Be(1);
    }

    [Fact]
    public void CreateHOTP_ValidParameters_ShouldSucceed()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var issuer = "Google";
        var accountName = "user@example.com";
        var secret = SecretKey.Create("encrypted_secret");

        // Act
        var account = OtpAccount.CreateHOTP(userId, issuer, accountName, secret);

        // Assert
        account.Should().NotBeNull();
        account.Type.Should().Be(OtpType.HOTP);
        account.Counter.Should().Be(0);
    }

    [Fact]
    public void IncrementCounter_HOTPAccount_ShouldIncreaseCounter()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var secret = SecretKey.Create("encrypted_secret");
        var account = OtpAccount.CreateHOTP(userId, "Issuer", "Account", secret);

        // Act
        account.IncrementCounter();

        // Assert
        account.Counter.Should().Be(1);
    }

    [Fact]
    public void IncrementCounter_TOTPAccount_ShouldThrowException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var secret = SecretKey.Create("encrypted_secret");
        var account = OtpAccount.CreateTOTP(userId, "Issuer", "Account", secret);

        // Act & Assert
        var action = () => account.IncrementCounter();
        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Delete_ShouldMarkAsDeleted()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var secret = SecretKey.Create("encrypted_secret");
        var account = OtpAccount.CreateTOTP(userId, "Issuer", "Account", secret);

        // Act
        account.Delete();

        // Assert
        account.IsDeleted.Should().BeTrue();
    }
}
