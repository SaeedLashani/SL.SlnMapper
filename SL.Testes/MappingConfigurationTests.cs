using System;
using System.Collections.Generic;
using SL.Domain.Models;
using Xunit;

public class MappingConfigurationTests
{
    [Fact]
    public void MappingConfiguration_ShouldInitializeCorrectly()
    {
        // Arrange
        var types = new List<TypeDefinition>
        {
            new TypeDefinition { Name = "TestType", Properties = new Dictionary<string, string> { { "Property1", "string" } } }
        };
        var mappings = new List<MappingDefinition>
        {
            new MappingDefinition
            {
                SourceType = "SourceType",
                TargetType = "TargetType",
                Fields = new Dictionary<string, FieldMapping> { { "Field1", new FieldMapping { TargetField = "TargetField1" } } }
            }
        };

        // Act
        var config = new MappingConfiguration
        {
            Types = types,
            Mappings = mappings
        };

        // Assert
        Assert.NotNull(config.Types);
        Assert.NotNull(config.Mappings);
        Assert.Single(config.Types);
        Assert.Single(config.Mappings);
    }
}
