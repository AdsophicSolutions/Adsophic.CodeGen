using System;
using System.Collections.Generic;
using System.Text;

namespace Adsophic.CodeGen.Models
{
    public class SchemaDefinition
    {
        public IEnumerable<ClassDefinition> ClassDefinitions { get; set; }
    }

    public class ClassDefinition
    {
        public string ClassName { get; set; }
        public string ControllerName => $"{ClassName}Controller";
        public string Namespace { get; set; }
        public bool GenerateController { get; set; }
        public IEnumerable<PropertyDefinition> Properties { get; set; }
        public IEnumerable<MethodDefinition> Methods { get; set; }
    }

    public class PropertyDefinition
    {
        public string PropertyType { get; set; }
        public string Name { get; set; }
    }

    public class MethodDefinition
    {
        public string MethodName { get; set; }
        public string ReturnType { get; set; }
        public IEnumerable<ParameterDefinition> Parameters { get; set; }
    }

    public class ParameterDefinition
    {
        public string ParameterType { get; set; }
        public string VariableName { get; set; }
        
    }
}
