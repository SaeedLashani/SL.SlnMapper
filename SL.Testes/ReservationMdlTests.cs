using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SL.Domain.Models;
using Xunit;

public class ReservationMdlTests
{
    [Fact]
    public void ReservationMdl_ShouldBeValid_WhenIdIsGreaterThanZero()
    {
        // Arrange
        var reservation = new ReservationMdl { Id = 1, Date = DateTime.Now, CustomerName = "Saeed Lashani" };
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(reservation);

        // Act
        var isValid = Validator.TryValidateObject(reservation, validationContext, validationResults, true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(validationResults);
    }

    [Fact]
    public void ReservationMdl_ShouldBeInvalid_WhenIdIsZeroOrLess()
    {
        // Arrange
        var reservation = new ReservationMdl { Id = 0, Date = DateTime.Now, CustomerName = "Saeed Lashani" };
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(reservation);

        // Act
        var isValid = Validator.TryValidateObject(reservation, validationContext, validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(validationResults, v => v.ErrorMessage.Contains("greater than zero"));
    }
}
