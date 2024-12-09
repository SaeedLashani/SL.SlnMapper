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
        public async Task<MappingConfigurationMdl> LoadJsonFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Configuration file not found at path: {filePath}");
            var json = await File.ReadAllTextAsync(filePath);
            return JsonConvert.DeserializeObject<MappingConfigurationMdl>(json);
        }
    }
}
