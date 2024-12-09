using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SL.Application;
using SL.Application.InfrastructureInterfaces;
using SL.Infrastructure;
using SL.Application.Services.Mappers;
using SL.Application.Services.Mappers.Interfaces;
using SL.Domain.Models;

namespace SL.StartAppPresentation.Extentions
{
    public static class ServiceBinding
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddSingleton<MappingConfigurationMdl>();
            services.AddSingleton<IMapHandler, MapHandler>();
            services.AddTransient<IJsonRepo,JsonRepo>();
            services.AddTransient<IMapAlgorithm, DynamicMapper>();


            return services;
        }
    }
}
