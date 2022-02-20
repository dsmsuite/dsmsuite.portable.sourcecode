using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DsmSuite.DsmViewer.ViewModel.Main;

namespace DsmSuite.Viewer.Main
{
    public partial class MainWindow : Window
    {
        private MainViewModel? _mainViewModel;

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            DataContextChanged += MainWindow_DataContextChanged;
        }

        private void MainWindow_DataContextChanged(object? sender, System.EventArgs e)
        {
            _mainViewModel = DataContext as MainViewModel;
            if (_mainViewModel != null)
            {
                _mainViewModel.FileOpenRequested += OnFileOpenRequested;
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async void OnFileOpenRequested(object? sender, System.EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filters.Add(new FileDialogFilter() { Name = "Model", Extensions = { "dsm", "dsi" } });
            dialog.AllowMultiple = true;

            var result = await dialog.ShowAsync(this);
        }
    }
}
