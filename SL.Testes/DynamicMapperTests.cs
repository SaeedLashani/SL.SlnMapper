using System;
using System.Threading.Tasks;
using Moq;
using SL.Application.Services.Mappers;
using SL.Domain.Models;
using Xunit;

namespace SL.Testes
{
    public class DynamicMapperTests
    {
        [Fact]
        public void Execute_ShouldMapPropertiesCorrectly()
        {
            var config = new MappingConfigurationMdl
            {
                Types = new List<TypeDefinition>
                {
                    new TypeDefinition
                    {
                        Name = "GoogleReservation",
                        Properties = new Dictionary<string, string>
                        {
                            { "GoogleReservationId", "string" },
                            { "ReservationDate", "string" },
                            { "GuestName", "string" }
                        }
                    }
                },
                    Mappings = new List<MappingDefinition>
                {
                    new MappingDefinition
                    {
                        SourceType = "ReservationMdl",
                        TargetType = "GoogleReservation",
                        GenerateClassFlag = true,
                        Fields = new Dictionary<string, FieldMapping>
                        {
                            { "Id", new FieldMapping { TargetField = "GoogleReservationId", ConversionType = "IntToString" } },
                            { "Date", new FieldMapping { TargetField = "ReservationDate", ConversionType = "DateTimeToString", Format = "yyyy-MM-dd" } },
                            { "CustomerName", new FieldMapping { TargetField = "GuestName" } }
                        }
                    }
                }
            };

            var mapper = new DynamicMapper();

            var source = new ReservationMdl
            {
                Id = 123,
                Date = new DateTime(2024, 11, 15),
                CustomerName = "Saeed Lashani"
            };

            // Act
            var result = mapper.Execute(source, "ReservationMdl", "GoogleReservation", config);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("123", result.GetType().GetProperty("GoogleReservationId")?.GetValue(result));
            Assert.Equal("2024-11-15", result.GetType().GetProperty("ReservationDate")?.GetValue(result));
            Assert.Equal("Saeed Lashani", result.GetType().GetProperty("GuestName")?.GetValue(result));

        }
    }
}
