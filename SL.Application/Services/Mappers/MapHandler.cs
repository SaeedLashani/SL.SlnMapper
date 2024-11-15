using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using SL.Application.InfrastructureInterfaces;
using SL.Application.Services.Mappers.Interfaces;


namespace SL.Application.Services.Mappers
{
    public class MapHandler : IMapHandler
    {
        private readonly IJsonRepo _jsonRepo;
        private readonly IMapAlgorithm _mapAlgorithm;
        private static readonly ILog log = LogManager.GetLogger(typeof(MapHandler));

        public MapHandler(IJsonRepo jsonRepo, IMapAlgorithm mapAlgorithm)
        {
            _jsonRepo = jsonRepo;
            _mapAlgorithm = mapAlgorithm;
        }

        public async Task<object> Map(object source, string sourceType, string targetType)
        {
            try
            {
                string filePath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "Config", "mapping-config.json");
                if (!File.Exists(filePath))
                    throw new FileNotFoundException($"Configuration file not found at path: {filePath}");

                var config = await _jsonRepo.LoadJsonFileAsync(filePath);

                //IMapAlgorithm mapAlgorithm = new DynamicMapper(config);

                return _mapAlgorithm.Execute(source, sourceType, targetType, config);

            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                throw new Exception("Unexpected error happened please check the logs");
            }
        }
    }
}
