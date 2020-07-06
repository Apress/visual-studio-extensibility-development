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
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Converters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace CodeGeneratorConnectedService
{
    [ConnectedServiceProviderExport(Constants.ProviderId, SupportsUpdate = true)]
    public class Provider : ConnectedServiceProvider
    {
        public Provider()
        {
            this.Category = Constants.Category;
            this.CreatedBy = Constants.CreatedBy;
            this.Description = Constants.Description;
            this.DescriptiveName = Constants.Name;
            this.Icon = new BitmapImage() { UriSource = new Uri("GenerateFile.png", UriKind.Relative) };
            this.Id = Constants.ProviderId;
            this.Name = Constants.Name;
            this.SupportsUpdate = true;
            this.Version = Assembly.GetExecutingAssembly().GetName().Version;
        }
                     
        public override Task<ConnectedServiceConfigurator> CreateConfiguratorAsync(ConnectedServiceProviderContext context)
        {
            return Task.FromResult<ConnectedServiceConfigurator>(new Wizard(context));
        }
    }
}
