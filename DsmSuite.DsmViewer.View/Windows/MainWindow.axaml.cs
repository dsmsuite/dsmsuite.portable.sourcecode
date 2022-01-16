using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DsmSuite.Viewer.View.UserControls;

namespace DsmSuite.Viewer.View.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
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
