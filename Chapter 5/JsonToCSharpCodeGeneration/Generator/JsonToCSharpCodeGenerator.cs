using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using NJsonSchema.CodeGeneration.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JsonToCSharpCodeGeneration
{
    [Guid("f0ff9543-4996-4be8-9061-c57131998819")]
    public class JsonToCSharpCodeGenerator : BaseCodeGeneratorWithSite
    {
        public const string Name = nameof(JsonToCSharpCodeGenerator);

        public const string Description = "Generates the C# class from JSON file";

        public override string GetDefaultExtension()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var item = (ProjectItem)GetService(typeof(ProjectItem));
            var ext = Path.GetExtension(item?.FileNames[1]);
            return $".cs";
        }

        protected override byte[] GenerateCode(string inputFileName, string inputFileContent)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            string document = string.Empty;
            try
            {
                document = ThreadHelper.JoinableTaskFactory.Run(async () =>
               {
                   var text = File.ReadAllText(inputFileName); // Alternatively, you can also use inputFileContent directly.
                   var schema = NJsonSchema.JsonSchema.FromSampleJson(text);
                   var generator = new CSharpGenerator(schema);
                   return await System.Threading.Tasks.Task.FromResult(generator.GenerateFile());
               });
            }
            catch (Exception exception)
            {
                // Write in output window
                var outputWindowPane = this.GetService(typeof(SVsGeneralOutputWindowPane)) as IVsOutputWindowPane;
                if (outputWindowPane != null)
                {
                    outputWindowPane.OutputString($"An exception occurred while generating code {exception.ToString()}");
                }

                // Show in error list
                this.GeneratorErrorCallback(false, 1, $"An exception occurred while generating code {exception.ToString()}", 1, 1);
                this.ErrorList.ForceShowErrors();
            }

            return Encoding.UTF8.GetBytes(document);
        }
    }
}
