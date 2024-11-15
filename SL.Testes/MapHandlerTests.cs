using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL.Testes
{
    using Moq;
    using System;
    using System.Threading.Tasks;
    using SL.Application.Services.Mappers;
    using SL.Application.InfrastructureInterfaces;
    using SL.Domain.Models;
    using Xunit;
    using SL.Application.Services.Mappers.Interfaces;

    public class MapHandlerTests
    {
        private readonly Mock<IJsonRepo> _jsonRepoMock = new Mock<IJsonRepo>();
        private readonly Mock<IMapAlgorithm> _mapAlgorithmMock = new Mock<IMapAlgorithm>();

        [Fact]
        public async Task Map_ShouldCallExecuteOnMapAlgorithm_WithCorrectParameters()
        {
            // Arrange
            var source = new ReservationMdl { Id = 123, Date = DateTime.Now, CustomerName = "Saeed Lashani" };
            var config = new MappingConfiguration();

            _jsonRepoMock.Setup(repo => repo.LoadJsonFileAsync(It.IsAny<string>())).ReturnsAsync(config);
            _mapAlgorithmMock.Setup(m => m.Execute(source, "Models.ReservationMdl", "GoogleReservation", config)).Returns(new object());

            var mapHandler = new MapHandler(_jsonRepoMock.Object,new DynamicMapper());

            // Act
            await mapHandler.Map(source, "Models.ReservationMdl", "GoogleReservation");

            // Assert
            _mapAlgorithmMock.Verify(m => m.Execute(source, "Models.ReservationMdl", "GoogleReservation", config), Times.Once);
        }

        [Fact]
        public async Task Map_ShouldThrowException_WhenConfigFileNotFound()
        {
            // Arrange
            _jsonRepoMock.Setup(repo => repo.LoadJsonFileAsync(It.IsAny<string>())).ThrowsAsync(new FileNotFoundException());
            var mapHandler = new MapHandler(_jsonRepoMock.Object, new DynamicMapper());

            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() => mapHandler.Map(new ReservationMdl(), "Models.ReservationMdl", "GoogleReservation"));
        }
    }

}
