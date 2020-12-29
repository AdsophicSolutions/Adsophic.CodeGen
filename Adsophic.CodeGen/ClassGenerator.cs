using Adsophic.CodeGen.API;
using Adsophic.CodeGen.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Adsophic.CodeGen
{
    public class ClassGenerator
    {
        private RazorLight.RazorLightEngine engine;
        private readonly Dictionary<TemplateType, string> templateDefinitions = new Dictionary<TemplateType, string>();
        private readonly ICodeFormatter codeFormatter;

        private ClassGenerator(ICodeFormatter codeFormatter)
        {
            Initialize();
            this.codeFormatter = codeFormatter;
        }

        private static readonly Lazy<ClassGenerator> instance = new Lazy<ClassGenerator>(() => new ClassGenerator(new CodeFormatter()));
        public static ClassGenerator Instance { get { return instance.Value; } }

        public void Generate(SchemaDefinition schemaDefinition, string outputPath)
        {
            if((schemaDefinition.ClassDefinitions?.Count() ?? 0) == 0) return;
            foreach(var classDefinition in schemaDefinition.ClassDefinitions)
            {
                GenerateClass(classDefinition, outputPath);
                if (classDefinition.GenerateController)
                    GenerateController(classDefinition, outputPath);
            }
        }

        public void GenerateClass(ClassDefinition classDefinition, string outputPath)
        {
            Task.Run(async () =>
            {
                string classString = await Generate(classDefinition, TemplateType.Class);
                Console.WriteLine($"Started writing class {classDefinition.ClassName} to " +
                    $"{(string.IsNullOrEmpty(outputPath) ? "current directory" : outputPath)}");
                if (codeFormatter != null) classString = codeFormatter.Format(classString);
                File.WriteAllText(Path.Combine(outputPath, $"{classDefinition.ClassName}.cs"), classString);
                
                Console.WriteLine($"Completed writing class {classDefinition.ClassName} to " +
                    $"{(string.IsNullOrEmpty(outputPath) ? "current directory" : outputPath)}");
            }).Wait();
        }

        public void GenerateController(ClassDefinition classDefinition, string outputPath)
        {
            Task.Run(async () =>
            {
                string controllerString = await Generate(classDefinition, TemplateType.Controller);
                Console.WriteLine($"Started writing controller {classDefinition.ControllerName} to " +
                    $"{(string.IsNullOrEmpty(outputPath) ? "current directory" : outputPath)}");
                if (codeFormatter != null) controllerString = codeFormatter.Format(controllerString);
                File.WriteAllText(Path.Combine(outputPath, $"{classDefinition.ControllerName}.cs"), controllerString);
                Console.WriteLine($"Completed writing controller {classDefinition.ControllerName} to " +
                    $"{(string.IsNullOrEmpty(outputPath) ? "current directory" : outputPath)}");
            }).Wait();
        }

        private async Task<string> Generate(ClassDefinition definition, TemplateType templateType)
        {
            // Try to find template.
            var cached = engine.TemplateCache.RetrieveTemplate(templateType.ToString());
            if (cached.Success)
            {
                // If template exists render template
                return await engine.RenderTemplateAsync(cached.Template.TemplatePageFactory(), definition);
            }

            // Compile and generate template
            return await engine.CompileRenderAsync(templateType.ToString(),
                templateDefinitions[templateType], definition);
        }

        private void Initialize()
        {
            // First create engine
            engine = new RazorLight.RazorLightEngineBuilder()
                //.UseEmbeddedResourcesProject(typeof(Program))
                .UseMemoryCachingProvider()
                .Build();

            templateDefinitions.Add(TemplateType.Class, 
                GetResourceAsString("Adsophic.CodeGen.Templates.classGeneration.cshtml"));
            templateDefinitions.Add(TemplateType.Controller,
                GetResourceAsString("Adsophic.CodeGen.Templates.controllerGeneration.cshtml"));
        }
                
        private static string GetResourceAsString(string resourceName)
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();

                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine($"Resource {resourceName} could not be loaded. Exception {e.Message}");
                throw;
            }
        }

        internal enum TemplateType
        {
            Class,
            Controller
        }
    }
}
