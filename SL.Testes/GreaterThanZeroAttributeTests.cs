using System.ComponentModel.DataAnnotations;
using SL.Domain.Validation;
using Xunit;

public class GreaterThanZeroAttributeTests
{
    [Fact]
    public void GreaterThanZeroAttribute_ShouldPass_WhenValueIsGreaterThanZero()
    {
        // Arrange
        var attribute = new GreaterThanZeroAttribute();
        var value = 1;

        // Act
        var result = attribute.GetValidationResult(value, new ValidationContext(value));

        // Assert
        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void GreaterThanZeroAttribute_ShouldFail_WhenValueIsZeroOrLess()
    {
        // Arrange
        var attribute = new GreaterThanZeroAttribute();
        var value = 0;

        // Act
        var result = attribute.GetValidationResult(value, new ValidationContext(value));

        // Assert
        Assert.NotNull(result);
        Assert.Equal("The Int32 field must be greater than zero.", result.ErrorMessage);
    }
}
