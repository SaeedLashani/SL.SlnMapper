using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SL.Domain.Models;

namespace SL.Application.Services.Mappers.Interfaces
{
    public interface IMapAlgorithm
    {
        object Execute(object source, string sourceType, string targetType, MappingConfiguration config);
    }
}
