using Xunit;
using FluentAssertions;
using SmartOTP.Domain.ValueObjects;

namespace SmartOTP.Domain.Tests.ValueObjects;

public class OtpCodeTests
{
    [Theory]
    [InlineData("123456", 30)]
    [InlineData("12345678", 60)]
    public void Create_ValidCode_ShouldSucceed(string code, int validityPeriod)
    {
        // Act
        var otpCode = OtpCode.Create(code, validityPeriod);

        // Assert
        otpCode.Should().NotBeNull();
        otpCode.Code.Should().Be(code);
        otpCode.ValidityPeriodSeconds.Should().Be(validityPeriod);
    }

    [Theory]
    [InlineData("")]
    [InlineData("12345")] // Too short
    [InlineData("123456789")] // Too long
    [InlineData("12345a")] // Contains letter
    public void Create_InvalidCode_ShouldThrowException(string code)
    {
        // Act & Assert
        var action = () => OtpCode.Create(code);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void IsValid_WithinValidityPeriod_ShouldReturnTrue()
    {
        // Arrange
        var otpCode = OtpCode.Create("123456", 30);

        // Act
        var isValid = otpCode.IsValid();

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void GetRemainingSeconds_ShouldReturnPositiveValue()
    {
        // Arrange
        var otpCode = OtpCode.Create("123456", 30);

        // Act
        var remaining = otpCode.GetRemainingSeconds();

        // Assert
        remaining.Should().BeGreaterThan(0);
        remaining.Should().BeLessThanOrEqualTo(30);
    }
}
