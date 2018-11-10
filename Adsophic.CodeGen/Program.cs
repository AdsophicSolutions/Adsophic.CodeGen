using Adsophic.CodeGen.Models;
using CommandLine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Adsophic.CodeGen
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Adsophic.CodeGen running...");
            //string json = Testing.DefinitionExamples.GetSampleSchemaDefinitionJSON();
            RunWithArguments(args);
        }

        static void RunWithArguments(string [] args)
        {
            Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed(options => RunOptionsAndReturnExitCode(options))
                .WithNotParsed((errors) => HandleParseError(errors));
        }

        private static void HandleParseError(IEnumerable<Error> errors)
        {
            Console.WriteLine("Could not parse command line arguments...");            
            Console.WriteLine("Errors:");

            Console.WriteLine(string.Join(Environment.NewLine, errors.Select(e => e.Tag.ToString())));
            Console.WriteLine("Quitting...");
        }

        private static void RunOptionsAndReturnExitCode(CommandLineOptions options)
        {
            var errors = options.Validate().ToArray();
            if(errors.Length != 0)
            {
                Console.WriteLine("Found errors in command line arguments...");
                Console.WriteLine("Errors:");

                Console.WriteLine(string.Join(Environment.NewLine, errors));
                Console.WriteLine("Quitting...");
                return;
            }

            GenerateClass(options);
        }

        static void GenerateClass(CommandLineOptions options)
        {                        
            var schemaDefinition = 
                JsonConvert.DeserializeObject<SchemaDefinition>(File.ReadAllText(options.DefinitionFilePath));

            ClassGenerator.Instance.Generate(schemaDefinition, options.OutputPath);
            
            //ClassGenerator.Instance.GenerateClass(classDefinition);
        }
    }
}
