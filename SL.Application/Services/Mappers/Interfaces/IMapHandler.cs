using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL.Application.Services.Mappers.Interfaces
{
    public interface IMapHandler
    {
        Task<object> Map(object source, string sourceType, string targetType);
    }
}
