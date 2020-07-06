namespace DisplayInfoBar
{
    using Microsoft.VisualStudio.Shell;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for ToolWindowWithInfoBarControl.
    /// </summary>
    public partial class ToolWindowWithInfoBarControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolWindowWithInfoBarControl"/> class.
        /// </summary>
        public ToolWindowWithInfoBarControl()
        {
            this.InitializeComponent();

            // Define the text to be displayed in Infobar.
            var text = "Welcome to Chapter 5. Are you liking it?";
            // Show in main window
            InfoBarService.Instance.ShowInfoBar(text);
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            InfoBarService.Instance.ShowInfoBar($"This info bar is invoked from tool window button. Are you liking it?", ToolWindowWithInfoBarCommand.ToolWindow);
        }
    }
}