////THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
////IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
////FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
////AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
////LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
////OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
////SOFTWARE.
////April 2020
////This project is just for illustration purposes and is not complete

using Microsoft.VisualStudio.ConnectedServices;
using NJsonSchema.CodeGeneration.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGeneratorConnectedService
{
    [ConnectedServiceHandlerExport(Constants.ProviderId, AppliesTo = "CSharp")]
    public class Handler : ConnectedServiceHandler
    {
        public override async Task<AddServiceInstanceResult> AddServiceInstanceAsync(ConnectedServiceHandlerContext context, CancellationToken ct)
        {
            var instance = (Instance)context.ServiceInstance;
            await context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, $"Generating code for {instance.JSONPath}");
            var csharpFilepath = await GenerateCSharpFileAsync(context, instance);
            await context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, $"Generated {Path.GetFileName(csharpFilepath)}");
            var folderName = context.ServiceInstance.Name;           
            var result = new AddServiceInstanceResult(folderName, null);
            return result;
        }

        public override async Task<UpdateServiceInstanceResult> UpdateServiceInstanceAsync(ConnectedServiceHandlerContext context, CancellationToken cancellationToken)
        {
            var instance = (Instance)context.ServiceInstance;
            await context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, $"Re-generating code for {instance.JSONPath}");
            var csharpFilePath = await GenerateCSharpFileAsync(context, instance);
            await context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, $"Re-generated code based on {csharpFilePath}");
            return await base.UpdateServiceInstanceAsync(context, cancellationToken);
        }

        private async Task<string> GenerateCSharpFileAsync(ConnectedServiceHandlerContext context, Instance instance)
        {
            var serviceFolder = instance.Name;
            string jsonPath = instance.JSONPath;

            var rootFolder = context.HandlerHelper.GetServiceArtifactsRootFolder();
            var folderPath = context.ProjectHierarchy.GetProject().GetServiceFolderPath(rootFolder);

            var text = File.ReadAllText(jsonPath);
            var schema = NJsonSchema.JsonSchema.FromSampleJson(text);
            var generator = new CSharpGenerator(schema);
            var result = await System.Threading.Tasks.Task.FromResult(generator.GenerateFile());
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(jsonPath);
            var fileName = $"{fileNameWithoutExtension}.cs";
            var generatedFullPath = Path.Combine(folderPath, fileName);
            await context.HandlerHelper.AddFileAsync(generatedFullPath, fileName);
            return generatedFullPath;
        }
    }
}
