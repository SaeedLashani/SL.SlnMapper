using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL.Domain.Models
{
    public class MappingConfiguration
    {
        public required List<TypeDefinition> Types { get; set; }
        public required List<MappingDefinition> Mappings { get; set; }
    }

    public class TypeDefinition
    {
        public required string Name { get; set; }
        public required Dictionary<string, string> Properties { get; set; }
    }

    public class MappingDefinition
    {
        public required string SourceType { get; set; }
        public required string TargetType { get; set; }
        public bool? GenerateClassFlag { get; set; }
        public required Dictionary<string, FieldMapping> Fields { get; set; }
    }

    public class FieldMapping
    {
        public required string TargetField { get; set; }
        public string? ConversionType { get; set; }
        public string? Format { get; set; }
    }
}
