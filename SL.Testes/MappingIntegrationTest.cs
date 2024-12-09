using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using SL.Application.InfrastructureInterfaces;
using SL.Application.Services.Mappers.Interfaces;
using SL.Application.Services.Mappers;
using SL.Domain.Models;

namespace SL.Testes
{
    public class MappingIntegrationTest
    {
        [Fact]
        public async Task Should_Map_Source_To_Target_Using_Mock_Successfully()
        {
            // Arrange
            var mockJsonRepo = new Mock<IJsonRepo>();
            var mockMapper = new Mock<IMapAlgorithm>();

            // Mocked MappingConfiguration
            var mockConfig = new MappingConfigurationMdl
            {
                Types = new List<TypeDefinition>
                {
                    new TypeDefinition
                    {
                        Name = "TargetType",
                        Properties = new Dictionary<string, string>
                        {
                            { "Id", "Int32" },
                            { "FullName", "String" }
                        }
                    }
                },
                        Mappings = new List<MappingDefinition>
                {
                    new MappingDefinition
                    {
                        SourceType = "SourceType",
                        TargetType = "TargetType",
                        GenerateClassFlag = false,
                        Fields = new Dictionary<string, FieldMapping>
                        {
                            { "Id", new FieldMapping { TargetField = "Id", ConversionType = null, Format = null } },
                            { "Name", new FieldMapping { TargetField = "FullName", ConversionType = null, Format = null } }
                        }
                    }
                }
            };

            // Mock JsonRepo to return the mocked configuration
            mockJsonRepo
                .Setup(repo => repo.LoadJsonFileAsync(It.IsAny<string>()))
                .ReturnsAsync(mockConfig);

            // Mock MapAlgorithm to return the expected target object
            mockMapper
                .Setup(m => m.Execute(It.IsAny<object>(), "SourceType", "TargetType", mockConfig))
                .Returns(new { Id = 1, FullName = "Test" });

            var mapHandler = new MapHandler(mockJsonRepo.Object, mockMapper.Object);

            // Act
            var result = await mapHandler.Map(new { Id = 1, Name = "Test" }, "SourceType", "TargetType");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.GetType().GetProperty("Id").GetValue(result));
            Assert.Equal("Test", result.GetType().GetProperty("FullName").GetValue(result));
        }
    }
}
