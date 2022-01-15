using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DsmSuite.Viewer.View.Editing
{
    public partial class SnapshotCreateDialog : Window
    {
        public SnapshotCreateDialog()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
