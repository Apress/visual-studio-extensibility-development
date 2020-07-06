using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PropertiesToolWindow
{
    [DisplayName("Tool Window Data")]
    public class ToolWindowData
    {
        [DisplayName("DTE Instance")]
        [Category("General")]
        [Description("The DTE Instance")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public DTE DTE { get; set; }

        [DisplayName("Async Package")]
        [Category("General")]
        [Description("The Package")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public AsyncPackage Package { get; set; }

        [DisplayName("Text Box")]
        [Category("General")]
        [Description("The TextBox")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public TextBox TextBox { get; set; }
    }
}
