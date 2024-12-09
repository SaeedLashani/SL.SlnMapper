using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;
using SL.Application.Services.Mappers.Interfaces;
using SL.Domain.Models;

namespace SL.Application.Services.Mappers
{
    public class DynamicMapper : IMapAlgorithm
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DynamicMapper));


        public object Execute(object source, string sourceType, string targetType, MappingConfigurationMdl config)
        {
            Validation(config);

            var mapping = FindMapping(config, sourceType, targetType);
            var targetClassType = GetTargetClassType(mapping, config, targetType);

            var targetInstance = Activator.CreateInstance(targetClassType)
                               ?? throw new InvalidOperationException("Target instance could not be created.");

            foreach (var field in mapping.Fields)
                MapProperty(source, targetInstance, targetClassType, field);

            return targetInstance;
        }

        private object ApplyConversion(object value, string conversionType, string format)
        {
            if (value == null || string.IsNullOrEmpty(conversionType))
                return value;

            return conversionType switch
            {
                "DateTimeToString" => ((DateTime)value).ToString(format),
                "StringToDateTime" => DateTime.ParseExact((string)value, format, CultureInfo.InvariantCulture),
                "IntToString" => value.ToString(),
                "StringToInt" => int.Parse((string)value),
                _ => throw new InvalidOperationException($"Unknown conversion type: {conversionType}")
            };
        }
        
        private void Validation(MappingConfigurationMdl config)
        {
            if (config is null)
                throw new ArgumentNullException(nameof(config), "Configuration cannot be null.");
            if (config.Mappings == null || !config.Mappings.Any())
                throw new InvalidOperationException($"No mappings List found in config file");
            if (config.Types == null || !config.Types.Any())
                throw new InvalidOperationException($"No Types List found in config file");
        }

        private MappingDefinition FindMapping(MappingConfigurationMdl config, string sourceType, string targetType)
        {
            var mapping = config.Mappings.FirstOrDefault(m => m.SourceType == sourceType && m.TargetType == targetType);
            if (mapping == null)
                throw new InvalidOperationException($"No mapping found for {sourceType} to {targetType}.");

            if (mapping.GenerateClassFlag == null)
                throw new InvalidOperationException($"GenerateClassFlag is not set for mapping {sourceType} to {targetType}.");

            return mapping;
        }

        private Type GetTargetClassType(MappingDefinition mapping, MappingConfigurationMdl config, string targetType)
        {
            if (mapping.GenerateClassFlag == true)
            {
                var targetTypeDef = config.Types.FirstOrDefault(t => t.Name == targetType)
                                    ?? throw new InvalidOperationException($"Target type definition '{targetType}' not found in configuration.");
                return DynamicTypeBuilder.CreateType(targetTypeDef);
            }

            var type = Type.GetType($"SL.Domain.{targetType},SL.Domain")
                       ?? throw new InvalidOperationException($"Target type '{targetType}' could not be resolved.");
            return type;
        }

        private void MapProperty(object source, object targetInstance, Type targetClassType, KeyValuePair<string, FieldMapping> field)
        {
            try
            {
                var sourceProperty = source.GetType().GetProperty(field.Key);
                var targetProperty = targetClassType.GetProperty(field.Value.TargetField);

                if (sourceProperty == null || targetProperty == null)
                    throw new ArgumentNullException($"Failed to set property '{field.Value.TargetField}' on '{targetClassType.Name}' due to one of them is null.");

                var value = sourceProperty.GetValue(source);
                var convertedValue = ApplyConversion(value, field.Value.ConversionType, field.Value.Format);
                targetProperty.SetValue(targetInstance, convertedValue);
            }
            catch (ArgumentException ex)
            {
                throw new InvalidOperationException($"Failed to set property '{field.Value.TargetField}' on '{targetClassType.Name}' due to type mismatch.");
            }
            catch (TargetInvocationException ex)
            {
                throw new InvalidOperationException($"Failed to set property '{field.Value.TargetField}' on '{targetClassType.Name}' due to an invocation error.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to set property '{field.Value.TargetField}' on '{targetClassType.Name}' due to an unexpected error.");
            }
        }
    }
}
