using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DsmSuite.Viewer.View.Matrix
{
    public partial class MatrixMetricsSelectorView : UserControl
    {
        public MatrixMetricsSelectorView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
