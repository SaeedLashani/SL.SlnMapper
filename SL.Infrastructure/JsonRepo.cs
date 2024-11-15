using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SL.Domain.Models;
using SL.Application.InfrastructureInterfaces;
using log4net;
using SL.Application.Services.Mappers;

namespace SL.Infrastructure
{
    public class JsonRepo : IJsonRepo
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(JsonRepo));
        public async Task<MappingConfiguration> LoadJsonFileAsync(string filePath)
        {
            try
            {
                var json = await File.ReadAllTextAsync(filePath);
                return JsonConvert.DeserializeObject<MappingConfiguration>(json);
            }
            catch (FileNotFoundException ex)
            {
                log.Error($"Json file not found at path: {filePath}", ex);
                throw;
            }
            catch (Exception ex)
            {
                log.Error("An unexpected error occurred in LoadJsonFileAsync Method", ex);
                throw;
            }
        }
    }
}
