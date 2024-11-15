using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SL.Domain.Models;

namespace SL.Application.InfrastructureInterfaces
{
    public interface IJsonRepo
    {
        Task<MappingConfiguration> LoadJsonFileAsync(string filePath);
    }
}
