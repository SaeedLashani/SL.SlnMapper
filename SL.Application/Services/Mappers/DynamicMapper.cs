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
        // private readonly MappingConfiguration _config;
        private static readonly ILog log = LogManager.GetLogger(typeof(DynamicMapper));

        //public DynamicMapper(MappingConfiguration config)
        //{
        //    _config = config;
        //}


        public object Execute(object source, string sourceType, string targetType, MappingConfiguration config)
        {
            try
            {
                Validation(config);

                var mapping = config.Mappings.Find(m => m.SourceType == sourceType && m.TargetType == targetType);
                if (mapping == null)
                    throw new InvalidOperationException($"No mapping found for {sourceType} to {targetType}");
                if (mapping.GenerateClassFlag == null)
                    throw new InvalidOperationException($"No GenerateClassFlag found for {sourceType} to {targetType}");


                Type targetClassType;
                if (mapping.GenerateClassFlag??false)
                {
                    var targetTypeDef = config.Types.Find(t => t.Name == targetType);
                    if (targetTypeDef == null)
                        throw new InvalidOperationException($"Target type '{targetType}' could not be found.");
                    targetClassType = DynamicTypeBuilder.CreateType(targetTypeDef);
                }
                else
                {
                    targetClassType = Type.GetType($"SL.Domain.{targetType},SL.Domain");
                    if (targetClassType == null)
                        throw new InvalidOperationException($"Target type '{targetType}' could not be found.");
                }

                // Create an instance of the target class
                var targetInstance = Activator.CreateInstance(targetClassType);

                // Map properties from source to target
                foreach (var field in mapping.Fields)
                {
                    var sourceProperty = source.GetType().GetProperty(field.Key);
                    var targetProperty = targetClassType.GetProperty(field.Value.TargetField);

                    if (sourceProperty != null && targetProperty != null)
                    {
                        try
                        {
                            var value = sourceProperty.GetValue(source);
                            var convertedValue = ApplyConversion(value, field.Value.ConversionType, field.Value.Format);
                            targetProperty.SetValue(targetInstance, convertedValue);
                        }
                        catch (ArgumentException ex)
                        {
                            // Handle cases where the type of convertedValue doesn't match the target property type
                            throw new InvalidOperationException($"Failed to set property '{field.Value.TargetField}' on '{targetClassType.Name}' due to type mismatch.");
                        }
                        catch (TargetInvocationException ex)
                        {
                            // Handle cases where the setter itself throws an exception
                            throw new InvalidOperationException($"Failed to set property '{field.Value.TargetField}' on '{targetClassType.Name}' due to an invocation error.");
                        }
                        catch (Exception ex)
                        {
                            // General exception handling for any other unexpected errors
                            throw new InvalidOperationException($"Failed to set property '{field.Value.TargetField}' on '{targetClassType.Name}' due to an unexpected error.");
                        }
                    }
                }

                return targetInstance;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private object ApplyConversion(object value, string conversionType, string format)
        {
            try
            {
                if (value == null || string.IsNullOrEmpty(conversionType)) return value;

                return conversionType switch
                {
                    "DateTimeToString" => ((DateTime)value).ToString(format),
                    "StringToDateTime" => DateTime.ParseExact((string)value, format, CultureInfo.InvariantCulture),
                    "IntToString" => value.ToString(),
                    "StringToInt" => int.Parse((string)value),
                    _ => throw new InvalidOperationException($"Unknown conversion type: {conversionType}")
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void Validation(MappingConfiguration config)
        {
            if(config is null) 
                throw new ArgumentNullException("config");
            if (config.Mappings == null)
                throw new InvalidOperationException($"No mappings List found in config file");
            if (config.Types == null)
                throw new InvalidOperationException($"No Types List found in config file");
        }

    }
}
