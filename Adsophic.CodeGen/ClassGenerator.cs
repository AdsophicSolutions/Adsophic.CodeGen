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
        private Dictionary<TemplateType, string> templateDefinitions = new Dictionary<TemplateType, string>();

        private ClassGenerator()
        {
            Initialize();
        }

        private static Lazy<ClassGenerator> instance = new Lazy<ClassGenerator>(() => new ClassGenerator());
        public static ClassGenerator Instance { get { return instance.Value; } }

        public void Generate(SchemaDefinition schemaDefinition, string outputhPath)
        {
            if((schemaDefinition.ClassDefinitions?.Count() ?? 0) == 0) return;
            foreach(var classDefinition in schemaDefinition.ClassDefinitions)
            {
                GenerateClass(classDefinition, outputhPath);
            }
        }

        public void GenerateClass(ClassDefinition classDefinition, string outputPath)
        {
            Task.Run(async () =>
            {
                string classString = await Generate(classDefinition);
                Console.WriteLine($"Started writing class {classDefinition.ClassName} to " +
                    $"{(string.IsNullOrEmpty(outputPath) ? "current directory" : outputPath)}");                
                File.WriteAllText(Path.Combine(outputPath, $"{classDefinition.ClassName}.cs"), classString);
                Console.WriteLine($"Completed writing class {classDefinition.ClassName} to " +
                    $"{(string.IsNullOrEmpty(outputPath) ? "current directory" : outputPath)}");
            }).Wait();
        }

        private async Task<string> Generate(ClassDefinition definition)
        {
            // Try to find template.
            var cached = engine.TemplateCache.RetrieveTemplate(TemplateType.Class.ToString());
            if (cached.Success)
            {
                // If template exists render template
                return await engine.RenderTemplateAsync(cached.Template.TemplatePageFactory(), definition);
            }

            // Compile and generate template
            return await engine.CompileRenderAsync(TemplateType.Class.ToString(),
                templateDefinitions[TemplateType.Class], definition);
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
            Class
        }
    }
}
