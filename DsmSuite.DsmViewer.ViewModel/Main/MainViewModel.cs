using System;
using System.Collections.Generic;
using System.IO;
using DsmSuite.DsmViewer.ViewModel.Common;
using DsmSuite.DsmViewer.ViewModel.Matrix;
using System.Linq;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Editing.Element;
using DsmSuite.DsmViewer.ViewModel.Editing.Snapshot;
using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.ViewModel.Settings;
using System.Reflection;
using ReactiveUI;
using System.Reactive.Linq;
using DsmSuite.DsmViewer.ViewModel.Lists.Element;
using DsmSuite.DsmViewer.ViewModel.Lists.Relation;
using DsmSuite.DsmViewer.ViewModel.Lists.Action;
using DsmSuite.DsmViewer.ViewModel.Search;

namespace DsmSuite.DsmViewer.ViewModel.Main
{
    public class MainViewModel : ReactiveViewModelBase, IMainViewModel
    {
        public void NotifyElementsReportReady(ElementListViewModelType viewModelType, IDsmElement selectedConsumer, IDsmElement selectedProvider)
        {
            ElementListViewModel elementListViewModel = new ElementListViewModel(viewModelType, _application, selectedConsumer, selectedProvider);
            ElementsReportReady?.Invoke(this, elementListViewModel);
        }

        public void NotifyRelationsReportReady(RelationsListViewModelType viewModelType, IDsmElement selectedConsumer, IDsmElement selectedProvider)
        {
            RelationListViewModel viewModel = new RelationListViewModel(viewModelType, _application, selectedConsumer, selectedProvider);
            RelationsReportReady?.Invoke(this, viewModel);
        }

        public event EventHandler<ElementEditViewModel> ElementEditStarted;

        public event EventHandler<SnapshotMakeViewModel> SnapshotMakeStarted;

        public event EventHandler<ElementListViewModel> ElementsReportReady;
        public event EventHandler<RelationListViewModel> RelationsReportReady;

        public event EventHandler<ActionListViewModel> ActionsVisible;

        public event EventHandler<SettingsViewModel> SettingsVisible;

        public event EventHandler ScreenshotRequested;

        private readonly IDsmApplication _application;
        private string _modelFilename;
        private string _title;
        private string _version;

        private bool _isModified;
        private bool _isLoaded;
        private readonly double _minZoom = 0.50;
        private readonly double _maxZoom = 2.00;
        private readonly double _zoomFactor = 1.25;

        private MatrixViewModel _activeMatrix;

        private readonly ProgressViewModel _progressViewModel;
        private string _redoText;
        private string _undoText;
        private string _selectedSortAlgorithm;
        private IndicatorViewMode _selectedIndicatorViewMode;

        public MainViewModel(IDsmApplication application)
        {
            _application = application;
            _application.Modified += OnModelModified;
            _application.ActionPerformed += OnActionPerformed;

            //OpenFileCommand = ReactiveCommand.Create(OpenFileExecute, OpenFileCanExecute);
            SaveFileCommand = ReactiveCommand.Create(SaveFileExecute, SaveFileCanExecute);

            HomeCommand = ReactiveCommand.Create(HomeExecute, HomeCanExecute);

            MoveUpElementCommand = ReactiveCommand.Create(MoveUpElementExecute, MoveUpElementCanExecute);
            MoveDownElementCommand = ReactiveCommand.Create(MoveDownElementExecute, MoveDownElementCanExecute);
            SortElementCommand = ReactiveCommand.Create(SortElementExecute, SortElementCanExecute);

            ToggleElementBookmarkCommand = ReactiveCommand.Create(ToggleElementBookmarkExecute, ToggleElementBookmarkCanExecute);

            ShowElementDetailMatrixCommand = ReactiveCommand.Create(ShowElementDetailMatrixExecute, ShowElementDetailMatrixCanExecute);
            ShowElementContextMatrixCommand = ReactiveCommand.Create(ShowElementContextMatrixExecute, ShowElementContextMatrixCanExecute);
            ShowCellDetailMatrixCommand = ReactiveCommand.Create(ShowCellDetailMatrixExecute, ShowCellDetailMatrixCanExecute);

            ZoomInCommand = ReactiveCommand.Create(ZoomInExecute, ZoomInCanExecute);
            ZoomOutCommand = ReactiveCommand.Create(ZoomOutExecute, ZoomOutCanExecute);
            ToggleElementExpandedCommand = ReactiveCommand.Create(ToggleElementExpandedExecute, ToggleElementExpandedCanExecute);

            UndoCommand = ReactiveCommand.Create(UndoExecute, UndoCanExecute);
            RedoCommand = ReactiveCommand.Create(RedoExecute, RedoCanExecute);

            AddElementCommand = ReactiveCommand.Create(AddElementExecute, AddElementCanExecute);
            ModifyElementCommand = ReactiveCommand.Create(ModifyElementExecute, ModifyElementCanExecute);
            DeleteElementCommand = ReactiveCommand.Create(DeleteElementExecute, DeleteElementCanExecute);
            ChangeElementParentCommand = ReactiveCommand.Create(MoveElementExecute, MoveElementCanExecute);

            MakeSnapshotCommand = ReactiveCommand.Create(MakeSnapshotExecute, MakeSnapshotCanExecute);
            ShowHistoryCommand = ReactiveCommand.Create(ShowHistoryExecute, ShowHistoryCanExecute);
            ShowSettingsCommand = ReactiveCommand.Create(ShowSettingsExecute, ShowSettingsCanExecute);

            TakeScreenshotCommand = ReactiveCommand.Create(TakeScreenshotExecute);

            _modelFilename = "";
            _version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            _title = $"DSM Viewer";

            _isModified = false;
            _isLoaded = false;

            _selectedSortAlgorithm = SupportedSortAlgorithms[0];

            _selectedIndicatorViewMode = IndicatorViewMode.Default;

            _progressViewModel = new ProgressViewModel();

            ActiveMatrix = new MatrixViewModel(this, _application, new List<IDsmElement>());
            ElementSearchViewModel = new ElementSearchViewModel(_application, null, null, null, true);
            ElementSearchViewModel.SearchUpdated += OnSearchUpdated;
        }

        private void OnSearchUpdated(object sender, EventArgs e)
        {
            SelectDefaultIndicatorMode();
            ActiveMatrix.Reload();
        }

        private void OnModelModified(object sender, bool e)
        {
            IsModified = e;
        }

        public IDsmElement SelectedConsumer => ActiveMatrix?.SelectedConsumer;

        public IDsmElement SelectedProvider => ActiveMatrix?.SelectedProvider;

        public ElementTreeItemViewModel SelectedProviderTreeItem => ActiveMatrix?.SelectedTreeItem;

        public MatrixViewModel ActiveMatrix
        {
            get { return _activeMatrix; }
            set { this.RaiseAndSetIfChanged(ref _activeMatrix, value); }
        }

        public ElementSearchViewModel ElementSearchViewModel { get; }

        private bool _isMetricsViewExpanded;

        public bool IsMetricsViewExpanded
        {
            get { return _isMetricsViewExpanded; }
            set { this.RaiseAndSetIfChanged(ref _isMetricsViewExpanded, value); }
        }

        public List<string> SupportedSortAlgorithms => _application.GetSupportedSortAlgorithms().ToList();

        public string SelectedSortAlgorithm
        {
            get { return _selectedSortAlgorithm; }
            set { this.RaiseAndSetIfChanged(ref _selectedSortAlgorithm, value); }
        }

        public List<IndicatorViewMode> SupportedIndicatorViewModes => Enum.GetValues(typeof(IndicatorViewMode)).Cast<IndicatorViewMode>().ToList();

        public IndicatorViewMode SelectedIndicatorViewMode
        {
            get { return _selectedIndicatorViewMode; }
            set { this.RaiseAndSetIfChanged(ref _selectedIndicatorViewMode, value); ActiveMatrix?.Reload(); }
        }

        public IReactiveCommand OpenFileCommand { get; }
        public IReactiveCommand SaveFileCommand { get; }
        public IReactiveCommand HomeCommand { get; }

        public IReactiveCommand MoveUpElementCommand { get; }
        public IReactiveCommand MoveDownElementCommand { get; }

        public IReactiveCommand ToggleElementBookmarkCommand { get; }

        public IReactiveCommand SortElementCommand { get; }
        public IReactiveCommand ShowElementDetailMatrixCommand { get; }
        public IReactiveCommand ShowElementContextMatrixCommand { get; }
        public IReactiveCommand ShowCellDetailMatrixCommand { get; }
        public IReactiveCommand ZoomInCommand { get; }
        public IReactiveCommand ZoomOutCommand { get; }
        public IReactiveCommand ToggleElementExpandedCommand { get; }
        public IReactiveCommand UndoCommand { get; }
        public IReactiveCommand RedoCommand { get; }

        public IReactiveCommand AddElementCommand { get; }
        public IReactiveCommand ModifyElementCommand { get; }
        public IReactiveCommand DeleteElementCommand { get; }
        public IReactiveCommand ChangeElementParentCommand { get; }

        public IReactiveCommand MakeSnapshotCommand { get; }
        public IReactiveCommand ShowHistoryCommand { get; }
        public IReactiveCommand ShowSettingsCommand { get; }
        public IReactiveCommand TakeScreenshotCommand { get; }

        public string ModelFilename
        {
            get { return _modelFilename; }
            set { this.RaiseAndSetIfChanged(ref _modelFilename, value); }
        }

        public bool IsModified
        {
            get { return _isModified; }
            set { this.RaiseAndSetIfChanged(ref _isModified, value); }
        }

        public bool IsLoaded
        {
            get { return _isLoaded; }
            set { this.RaiseAndSetIfChanged(ref _isLoaded, value); }
        }

        public string Title
        {
            get { return _title; }
            set { this.RaiseAndSetIfChanged(ref _title, value); }
        }

        public string Version
        {
            get { return _version; }
            set { this.RaiseAndSetIfChanged(ref _version, value); }
        }

        public ProgressViewModel ProgressViewModel => _progressViewModel;

        private async void OpenFileExecute(object parameter)
        {
            var progress = new Progress<ProgressInfo>(p =>
            {
                _progressViewModel.Update(p);
            });

            _progressViewModel.Action = "Reading";
            string filename = parameter as string;
            if (filename != null)
            {
                FileInfo fileInfo = new FileInfo(filename);

                switch (fileInfo.Extension)
                {
                    case ".dsm":
                        {
                            FileInfo dsmFileInfo = fileInfo;
                            await _application.OpenModel(dsmFileInfo.FullName, progress);
                            ModelFilename = dsmFileInfo.FullName;
                            Title = $"DSM Viewer - {dsmFileInfo.Name}";
                            IsLoaded = true;
                        }
                        break;
                    case ".dsi":
                        {
                            FileInfo dsiFileInfo = fileInfo;
                            FileInfo dsmFileInfo = new FileInfo(fileInfo.FullName.Replace(".dsi", ".dsm"));
                            await _application.AsyncImportDsiModel(dsiFileInfo.FullName, dsmFileInfo.FullName, false, true, progress);
                            ModelFilename = dsmFileInfo.FullName;
                            Title = $"DSM Viewer - {dsmFileInfo.Name}";
                            IsLoaded = true;
                        }
                        break;
                }
                ActiveMatrix = new MatrixViewModel(this, _application, GetRootElements());
            }
        }

        private bool OpenFileCanExecute(object parameter)
        {
            string fileToOpen = parameter as string;
            if (fileToOpen != null)
            {
                return File.Exists(fileToOpen);
            }
            else
            {
                return false;
            }
        }

        private async void SaveFileExecute()
        {
            var progress = new Progress<ProgressInfo>(p =>
            {
                _progressViewModel.Update(p);
            });

            _progressViewModel.Action = "Reading";
            await _application.SaveModel(ModelFilename, progress);
        }

        private IObservable<bool>? SaveFileCanExecute
        {
            get
            {
                return Observable.Return(_application.IsModified);
            }
        }

        private void HomeExecute()
        {
            IncludeAllInTree();
            ActiveMatrix.Reload();
        }

        private IEnumerable<IDsmElement> GetRootElements()
        {
            return new List<IDsmElement> { _application.RootElement };
        }

        private IObservable<bool>? HomeCanExecute
        {
            get
            {
                return Observable.Return(IsLoaded);
            }
        }

        private void SortElementExecute()
        {
            _application.Sort(SelectedProvider, SelectedSortAlgorithm);
        }

        private IObservable<bool>? SortElementCanExecute
        {
            get
            {
                return Observable.Return(_application.HasChildren(SelectedProvider));
            }
        }

        private void ShowElementDetailMatrixExecute()
        {
            ExcludeAllFromTree();
            IncludeInTree(SelectedProvider);
            ActiveMatrix.Reload();
        }

        private IObservable<bool>? ShowElementDetailMatrixCanExecute
        {
            get
            {
                return Observable.Return(true);
            }
        }

        private void ShowElementContextMatrixExecute()
        {
            ExcludeAllFromTree();
            IncludeInTree(SelectedProvider);

            foreach (IDsmElement consumer in _application.GetElementConsumers(SelectedProvider))
            {
                IncludeInTree(consumer);
            }

            foreach (IDsmElement provider in _application.GetElementProviders(SelectedProvider))
            {
                IncludeInTree(provider);
            }

            ActiveMatrix.Reload();
        }

        private IObservable<bool>? ShowElementContextMatrixCanExecute
        {
            get
            {
                return Observable.Return(true);
            }
        }

        private void ShowCellDetailMatrixExecute()
        {
            ExcludeAllFromTree();
            IncludeInTree(SelectedProvider);
            IncludeInTree(SelectedConsumer);

            ActiveMatrix.Reload();
        }

        private IObservable<bool>? ShowCellDetailMatrixCanExecute
        {
            get
            {
                return Observable.Return(true);
            }
        }

        private void MoveUpElementExecute()
        {
            _application.MoveUp(SelectedProvider);
        }

        private IObservable<bool>? MoveUpElementCanExecute
        {
            get
            {
                IDsmElement current = SelectedProvider;
                IDsmElement previous = _application.PreviousSibling(current);
                return Observable.Return((current != null) && (previous != null));
            }
        }

        private void MoveDownElementExecute()
        {
            _application.MoveDown(SelectedProvider);
        }

        private IObservable<bool>? MoveDownElementCanExecute
        {
            get
            {
                IDsmElement current = SelectedProvider;
                IDsmElement next = _application.NextSibling(current);
                return Observable.Return((current != null) && (next != null));
            }
        }

        private void ZoomInExecute()
        {
            if (ActiveMatrix != null)
            {
                ActiveMatrix.ZoomLevel *= _zoomFactor;
            }
        }

        private IObservable<bool>? ZoomInCanExecute
        {
            get
            {
                return Observable.Return(ActiveMatrix?.ZoomLevel < _maxZoom);
            }
        }

        private void ZoomOutExecute()
        {
            if (ActiveMatrix != null)
            {
                ActiveMatrix.ZoomLevel /= _zoomFactor;
            }
        }

        private IObservable<bool>? ZoomOutCanExecute
        {
            get
            {
                return Observable.Return(ActiveMatrix?.ZoomLevel > _minZoom);
            }
        }

        private void ToggleElementExpandedExecute()
        {
            ActiveMatrix.SelectTreeItem(ActiveMatrix.HoveredTreeItem);
            if ((SelectedProviderTreeItem != null) && (SelectedProviderTreeItem.IsExpandable))
            {
                SelectedProviderTreeItem.IsExpanded = !SelectedProviderTreeItem.IsExpanded;
            }

            ActiveMatrix.Reload();
        }

        private IObservable<bool>? ToggleElementExpandedCanExecute
        {
            get
            {
                return Observable.Return(true);
            }
        }

        public string UndoText
        {
            get { return _undoText; }
            set { this.RaiseAndSetIfChanged(ref _undoText, value); }
        }

        private void UndoExecute()
        {
            _application.Undo();
        }

        private IObservable<bool>? UndoCanExecute
        {
            get
            {
                return Observable.Return(_application.CanUndo());
            }
        }

        private void RedoExecute()
        {
            _application.Redo();
        }

        public string RedoText
        {
            get { return _redoText; }
            set { this.RaiseAndSetIfChanged(ref _redoText, value); }
        }

        private IObservable<bool>? RedoCanExecute
        {
            get
            {
                return Observable.Return(_application.CanRedo());
            }
        }

        private void SelectDefaultIndicatorMode()
        {
            SelectedIndicatorViewMode = string.IsNullOrEmpty(ElementSearchViewModel.SearchText) ? IndicatorViewMode.Default : IndicatorViewMode.Search;
        }

        private void OnActionPerformed(object sender, EventArgs e)
        {
            UndoText = $"Undo {_application.GetUndoActionDescription()}";
            RedoText = $"Redo {_application.GetRedoActionDescription()}";
            ActiveMatrix?.Reload();
        }

        private void AddElementExecute()
        {
            ElementEditViewModel elementEditViewModel = new ElementEditViewModel(ElementEditViewModelType.Add, _application, SelectedProvider);
            ElementEditStarted?.Invoke(this, elementEditViewModel);
        }

        private IObservable<bool>? AddElementCanExecute
        {
            get
            {
                return Observable.Return(true);
            }
        }

        private void ModifyElementExecute()
        {
            ElementEditViewModel elementEditViewModel = new ElementEditViewModel(ElementEditViewModelType.Modify, _application, SelectedProvider);
            ElementEditStarted?.Invoke(this, elementEditViewModel);
        }

        private IObservable<bool>? ModifyElementCanExecute
        {
            get
            {
                bool canExecute = false;
                if (SelectedProvider != null)
                {
                    canExecute = !SelectedProvider.IsRoot;
                }
                return Observable.Return(canExecute);
            }
        }

        private void DeleteElementExecute()
        {
            _application.DeleteElement(SelectedProvider);
        }

        private IObservable<bool>? DeleteElementCanExecute
        {
            get
            {
                bool canExecute = false;
                if (SelectedProvider != null)
                {
                    canExecute = !SelectedProvider.IsRoot;
                }
                return Observable.Return(canExecute);
            }
        }

        private void MoveElementExecute()
        {
            //Tuple<IDsmElement, IDsmElement, int> moveParameter = parameter as Tuple<IDsmElement, IDsmElement, int>;
            //if (moveParameter != null)
            //{
            //    _application.ChangeElementParent(moveParameter.Item1, moveParameter.Item2, moveParameter.Item3);
            //    // TODO: Fix CommandManager.InvalidateRequerySuggested();
            //}
        }

        private IObservable<bool>? MoveElementCanExecute
        {
            get
            {
                bool canExecute = false;
                if (SelectedProvider != null)
                {
                    canExecute = !SelectedProvider.IsRoot;
                }
                return Observable.Return(canExecute);
            }
        }

        private void ToggleElementBookmarkExecute()
        {
            if (SelectedProvider != null)
            {
                SelectedProvider.IsBookmarked = !SelectedProvider.IsBookmarked;
                ActiveMatrix?.Reload();
            }
        }

        private IObservable<bool>? ToggleElementBookmarkCanExecute
        {
            get
            {
                return Observable.Return(_selectedIndicatorViewMode == IndicatorViewMode.Bookmarks);
            }
        }

        private void MakeSnapshotExecute()
        {
            SnapshotMakeViewModel viewModel = new SnapshotMakeViewModel(_application);
            SnapshotMakeStarted?.Invoke(this, viewModel);
        }

        private IObservable<bool>? MakeSnapshotCanExecute
        {
            get
            {
                return Observable.Return(true);
            }
        }

        private void ShowHistoryExecute()
        {
            ActionListViewModel viewModel = new ActionListViewModel(_application);
            ActionsVisible?.Invoke(this, viewModel);
        }

        private IObservable<bool>? ShowHistoryCanExecute
        {
            get
            {
                return Observable.Return(true);
            }
        }

        private void ShowSettingsExecute()
        {
            SettingsViewModel viewModel = new SettingsViewModel(_application);
            SettingsVisible?.Invoke(this, viewModel);
            ActiveMatrix?.Reload();
        }

        private IObservable<bool>? ShowSettingsCanExecute
        {
            get 
            { 
                return Observable.Return(true);
            }
        }

        private void TakeScreenshotExecute()
        {
            ScreenshotRequested?.Invoke(this, EventArgs.Empty);
        }

        private void ExcludeAllFromTree()
        {
            UpdateChildrenIncludeInTree(_application.RootElement, false);
        }

        private void IncludeAllInTree()
        {
            UpdateChildrenIncludeInTree(_application.RootElement, true);
        }

        private void IncludeInTree(IDsmElement element)
        {
            UpdateChildrenIncludeInTree(element, true);
            UpdateParentsIncludeInTree(element, true);
        }

        private void UpdateChildrenIncludeInTree(IDsmElement element, bool included)
        {
            element.IsIncludedInTree = included;

            foreach (IDsmElement child in element.AllChildren)
            {
                UpdateChildrenIncludeInTree(child, included);
            }
        }

        private void UpdateParentsIncludeInTree(IDsmElement element, bool included)
        {
            IDsmElement current = element;
            do
            {
                current.IsIncludedInTree = included;
                current = current.Parent;
            } while (current != null);
        }
    }
}
