using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Adsophic.CodeGen
{
    public class CommandLineOptions
    {
        [Option('d', "definition", Required = true, HelpText = "Path to the configuration file")]
        public string DefinitionFilePath { get; set; }

        [Option('o', "output", Required = false, HelpText = "Root path where output files will be produced")]
        public string OutputPath { get; set; }
        
        internal IEnumerable<string> Validate()
        {
            var errors = new List<string>();
            if (!File.Exists(DefinitionFilePath))
                errors.Add($"Configuration file path {DefinitionFilePath} is invalid");

            if(!string.IsNullOrWhiteSpace(OutputPath) && 
                !Directory.Exists(OutputPath.Trim()))
            {
                try
                {
                    Directory.CreateDirectory(OutputPath);
                }
                catch(Exception e)
                {
                    errors.Add($"Output path {OutputPath} does not exist and cannot be created. Error {e.Message}");
                }
            }

            return errors;
        }
    }
}
