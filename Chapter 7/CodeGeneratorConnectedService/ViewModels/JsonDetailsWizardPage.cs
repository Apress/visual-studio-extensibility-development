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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorConnectedService.ViewModels
{
    public class JsonDetailsWizardPage : ConnectedServiceWizardPage
    {
        private readonly ConnectedServiceProviderContext context;

        private readonly IDictionary<string, object> metadata;

        private readonly string[] propertyNames = {
            nameof(JsonPath),
        };

        public string JsonPath
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        public JsonDetailsWizardPage(ConnectedServiceProviderContext context)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            metadata = new Dictionary<string, object>
            {
                {nameof(JsonPath), null}
            };

            this.context = context;
            
            Title = Constants.Name;
            Description = Constants.Description;
            Legend = "JSON";
            JsonPath = Path.Combine(Path.GetDirectoryName(this.context.ProjectHierarchy.GetProject().FullName?.ToString()), "appSettings.json");
            View = new Views.JsonDetailsWizardPage
            {
                DataContext = this
            };
        }

        protected override void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (propertyNames.Contains(propertyName))
            {
                HasErrors = !IsPageValid();
            }

            base.OnPropertyChanged(propertyName);
        }

        private bool IsPageValid()
        {
            if (string.IsNullOrWhiteSpace(JsonPath))
            {
                return false;
            }

            return true;
        }

        private void SetProperty<T>(T value, [CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
            {
                return;
            }

            if (!metadata.ContainsKey(propertyName))
            {
                metadata.Add(propertyName, value);
                OnPropertyChanged(propertyName);
            }
            else
            {
                var currentValue = (T)metadata[propertyName];
                if (!EqualityComparer<T>.Default.Equals(currentValue, value))
                {
                    metadata[propertyName] = value;
                    OnPropertyChanged(propertyName);
                }
            }
        }

        private T GetProperty<T>([CallerMemberName] string propertyName = null)
        {
            if (propertyName != null && metadata.ContainsKey(propertyName))
            {
                var currentValue = (T)metadata[propertyName];
                return currentValue;
            }

            return default;
        }
    }
}
