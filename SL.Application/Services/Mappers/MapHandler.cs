using SL.Application.InfrastructureInterfaces;
using SL.Application.Services.Mappers.Interfaces;
using SL.Application.Utils;
using SL.Domain.Models;


namespace SL.Application.Services.Mappers
{
    public class MapHandler : IMapHandler
    {
        private readonly IJsonRepo _jsonRepo;
        private readonly IMapAlgorithm _mapAlgorithm;

        public MapHandler(IJsonRepo jsonRepo, IMapAlgorithm mapAlgorithm)
        {
            _jsonRepo = jsonRepo;
            _mapAlgorithm = mapAlgorithm;
        }

        public async Task<object> Map(object source, string sourceType, string targetType)
        {
            try
            {
                ErrorHandler.LogInfo($"Starting mapping from {sourceType} to {targetType}.");

                var config = await PrepareConfig();

                var result = _mapAlgorithm.Execute(source, sourceType, targetType, config);


                return result;

            }
            catch (Exception ex)
            {
                var error = ErrorHandler.Handle(ex, null, false);
                string message = error.IsShowMessage ? error.Message : "Unexpected error happened please check the logs";

                throw new Exception(message);

            }
            finally
            {
                ErrorHandler.LogInfo($"Mapping completed from {sourceType} to {targetType}.");
            }
        }

        public async Task<MappingConfigurationMdl> PrepareConfig()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "mapping-config.json");
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Configuration file not found at path: {filePath}");

            return await _jsonRepo.LoadJsonFileAsync(filePath);
        }
    }
}
