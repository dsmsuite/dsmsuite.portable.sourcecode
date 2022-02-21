using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DsmSuite.DsmViewer.Application.Core;
using DsmSuite.DsmViewer.Model.Core;
using DsmSuite.DsmViewer.ViewModel.Editing.Element;
using DsmSuite.DsmViewer.ViewModel.Editing.Snapshot;
using DsmSuite.DsmViewer.ViewModel.Main;
using DsmSuite.DsmViewer.ViewModel.Settings;
using System.Reflection;
using System.ComponentModel;
using System;
using DsmSuite.DsmViewer.ViewModel.Lists.Action;
using DsmSuite.DsmViewer.ViewModel.Lists.Element;
using DsmSuite.DsmViewer.ViewModel.Lists.Relation;

namespace DsmSuite.Viewer.Main
{
    public partial class MainWindow : Window
    {
        private MainViewModel? _mainViewModel;
        //private ProgressWindow? _progressWindow;

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            DataContextChanged += MainWindow_DataContextChanged;
            Closing += MainWindow_Closing;
          
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void MainWindow_DataContextChanged(object? sender, EventArgs e)
        {
            _mainViewModel = DataContext as MainViewModel;
            if (_mainViewModel != null)
            {
                _mainViewModel.FileOpenRequested += OnFileOpenRequested;

                DsmModel model = new DsmModel("Viewer", Assembly.GetExecutingAssembly());
                DsmApplication application = new DsmApplication(model);
                _mainViewModel = new MainViewModel(application);
                _mainViewModel.ElementsReportReady += OnElementsReportReady;
                _mainViewModel.RelationsReportReady += OnRelationsReportReady;
                _mainViewModel.ProgressViewModel.BusyChanged += OnProgressViewModelBusyChanged;

                _mainViewModel.ElementEditStarted += OnElementEditStarted;

                _mainViewModel.SnapshotMakeStarted += OnSnapshotMakeStarted;

                _mainViewModel.ActionsVisible += OnActionsVisible;
                _mainViewModel.SettingsVisible += OnSettingsVisible;

                _mainViewModel.ScreenshotRequested += OnScreenshotRequested;
            }
        }

        private async void OnFileOpenRequested(object? sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filters.Add(new FileDialogFilter() { Name = "Model", Extensions = { "dsm", "dsi" } });
            dialog.AllowMultiple = false;

            string[]? files_to_be_opened = await dialog.ShowAsync(this);
            if ((files_to_be_opened != null) && (files_to_be_opened.Length == 1))
            {
                _mainViewModel?.OpenFile(files_to_be_opened[0]);
            }
        }

        private void MainWindow_Closing(object? sender, CancelEventArgs e)
        {

        }

        private void OnElementEditStarted(object? sender, ElementEditViewModel viewModel)
        {
            //ElementEditDialog view = new ElementEditDialog { DataContext = viewModel };
            //view.ShowDialog();
        }

        private void OnActionsVisible(object? sender, ActionListViewModel viewModel)
        {
            //ActionListView view = new ActionListView { DataContext = viewModel };
            //view.Show();
        }

        private void OnElementsReportReady(object? sender, ElementListViewModel e)
        {
            //ElementListView view = new ElementListView
            //{
            //    DataContext = e,
            //    Owner = this
            //};
            //view.Show();
        }

        private void OnRelationsReportReady(object? sender, RelationListViewModel e)
        {
            //RelationListView view = new RelationListView
            //{
            //    DataContext = e,
            //    Owner = this
            //};
            //view.Show();
        }

        private void OnProgressViewModelBusyChanged(object? sender, bool visible)
        {
            //if (visible)
            //{
            //    if (_progressWindow == null)
            //    {
            //        _progressWindow = new ProgressWindow
            //        {
            //            DataContext = _mainViewModel.ProgressViewModel,
            //            Owner = this
            //        };
            //        _progressWindow.ShowDialog();
            //    }
            //}
            //else
            //{
            //    _progressWindow.Close();
            //    _progressWindow = null;
            //}
        }

        private void OnSettingsVisible(object? sender, SettingsViewModel viewModel)
        {
            //SettingsView view = new SettingsView { DataContext = viewModel };
            //view.ShowDialog();
        }

        private void OnSnapshotMakeStarted(object? sender, SnapshotMakeViewModel viewModel)
        {
            //SnapshotCreateDialog view = new SnapshotCreateDialog { DataContext = viewModel };
            //view.ShowDialog();
        }

        private void OnScreenshotRequested(object? sender, System.EventArgs e)
        {
            //const int leftMargin = 5;
            //const int topMargin = 70;
            //const int bottomMargin = 2;
            //int width = (int)(Matrix.UsedWidth * _mainViewModel.ActiveMatrix.ZoomLevel) + leftMargin;
            //int height = (int)(Matrix.UsedHeight * _mainViewModel.ActiveMatrix.ZoomLevel) + topMargin;
            //RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            //renderTargetBitmap.Render(Matrix);
            //Int32Rect rect = new Int32Rect(leftMargin, topMargin, width - leftMargin, height - topMargin - bottomMargin);
            //CroppedBitmap croppedBitmap = new CroppedBitmap(renderTargetBitmap, rect);
            //Clipboard.SetImage(croppedBitmap);
        }
    }
}
