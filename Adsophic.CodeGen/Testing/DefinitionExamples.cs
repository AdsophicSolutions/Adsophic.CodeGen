using Adsophic.CodeGen.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Adsophic.CodeGen.Testing
{
    internal static class DefinitionExamples
    {
        public static string GetSampleSchemaDefinitionJSON()
        {
            return JsonConvert.SerializeObject(GetSampleSchemaDefinition());
        }
        public static SchemaDefinition GetSampleSchemaDefinition()
        {
            var schemaDefinition = new SchemaDefinition();
            var classDefinitions = new List<ClassDefinition>();
            schemaDefinition.ClassDefinitions = classDefinitions;

            var classDefinition = new ClassDefinition
            {
                ClassName = "MyTestClass",
                Namespace = "Adsophic.Autogeneration"
                
            };
            classDefinition.Properties = new PropertyDefinition[]
            {
                    new PropertyDefinition { PropertyType = "string", Name = "FirstName" },
                    new PropertyDefinition { PropertyType = "string", Name = "LastName" },
                    new PropertyDefinition { PropertyType = "string", Name = "Digits" },
            };
            classDefinition.Methods = new MethodDefinition[]
            {
                new MethodDefinition
                {
                    MethodName = "MyTestMethod",
                    ReturnType = "string",
                    Parameters = new ParameterDefinition []
                    {
                        new ParameterDefinition { ParameterType = "string", VariableName = "mystring"}
                    }
                }
            };
            classDefinitions.Add(classDefinition);

            Console.WriteLine("One class finished. Starting another");
            classDefinition = new ClassDefinition
            {
                ClassName = "MyOtherClass",
                Namespace = "Adsophic.Autogeneration"
            };
            classDefinition.Properties = new PropertyDefinition[]
            {
                    new PropertyDefinition { PropertyType = "string", Name = "FirstName" },
                    new PropertyDefinition { PropertyType = "int", Name = "Digits" },
            };
            classDefinitions.Add(classDefinition);
            

            return schemaDefinition;
        }
    }
}
