////THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
////IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
////FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
////AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
////LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
////OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
////SOFTWARE.
////April 2020
////This project is just for illustration purposes and is not complete

using CodeGeneratorConnectedService.ViewModels;
using Microsoft.VisualStudio.ConnectedServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorConnectedService
{
    class Wizard : ConnectedServiceWizard
    {
        private readonly ConnectedServiceProviderContext context;

        public Wizard(ConnectedServiceProviderContext context)
        {
            this.context = context;
            Pages.Add(new JsonDetailsWizardPage(context));

            foreach (var page in Pages)
            {
                page.PropertyChanged += this.OnPagePropertyChanged;
            }
        }

        private void OnPagePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.IsFinishEnabled = Pages.All(page => !page.HasErrors);
            this.IsNextEnabled = false;
        }

        public override Task<ConnectedServiceInstance> GetFinishedServiceInstanceAsync()
        {
            var instance = new Instance();
            instance.JSONPath = Pages.OfType<JsonDetailsWizardPage>().FirstOrDefault()?.JsonPath;
            return Task.FromResult<ConnectedServiceInstance>(instance);
        }
    }
}
