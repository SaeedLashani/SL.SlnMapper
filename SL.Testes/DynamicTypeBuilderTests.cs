using System;
using System.Collections.Generic;
using SL.Application.Services.Mappers;
using SL.Domain.Models;
using Xunit;

public class DynamicTypeBuilderTests
{
    [Fact]
    public void CreateType_ShouldGenerateTypeWithProperties()
    {
        // Arrange
        var typeDefinition = new TypeDefinition
        {
            Name = "TestType",
            Properties = new Dictionary<string, string> { { "Property1", "string" } }
        };

        // Act
        var generatedType = DynamicTypeBuilder.CreateType(typeDefinition);

        // Assert
        Assert.NotNull(generatedType);
        Assert.Equal("TestType", generatedType.Name);
        Assert.NotNull(generatedType.GetProperty("Property1"));
    }
}
