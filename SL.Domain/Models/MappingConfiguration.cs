using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL.Domain.Models
{
    public class MappingConfiguration
    {
        public List<TypeDefinition> Types { get; set; }
        public List<MappingDefinition> Mappings { get; set; }
    }

    public class TypeDefinition
    {
        public string Name { get; set; }
        public Dictionary<string, string> Properties { get; set; }
    }

    public class MappingDefinition
    {
        public string SourceType { get; set; }
        public string TargetType { get; set; }
        public bool? GenerateClassFlag { get; set; }
        public Dictionary<string, FieldMapping> Fields { get; set; }
    }

    public class FieldMapping
    {
        public string TargetField { get; set; }
        public string ConversionType { get; set; }
        public string Format { get; set; }
    }
}
