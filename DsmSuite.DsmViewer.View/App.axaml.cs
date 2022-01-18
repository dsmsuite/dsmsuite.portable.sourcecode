using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DsmSuite.Viewer.Main;
using DsmSuite.DsmViewer.ViewModel.Main;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Application.Core;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.Model.Core;
using System.Reflection;

namespace DsmSuite.Viewer.View
{
    public class App : Application
    {
        private IDsmModel? _model;
        private IDsmApplication? _application;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                _model = new DsmModel("Viewer", Assembly.GetExecutingAssembly());
                _application = new DsmApplication(_model);
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainViewModel(_application),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
