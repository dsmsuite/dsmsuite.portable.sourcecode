using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DsmSuite.Viewer.View.Lists
{
    public partial class ElementListView : Window
    {
        public ElementListView()
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
