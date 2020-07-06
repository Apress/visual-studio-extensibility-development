using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace PropertiesToolWindow
{
    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid(ToolWindowId)]
    public class ToolWindow : ToolWindowPane
    {
        internal const string ToolWindowId = "240c2f5f-f746-43d4-8d9e-44e144ed1f69";

        public ToolWindow(ToolWindowData data) : base()
        {
            this.Caption = "Automation Properties";
            this.BitmapImageMoniker = KnownMonikers.ListProperty;
            this.Content = new ToolWindowControl(data);
        }
    }
}