using System.ComponentModel;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;
using DsmSuite.DsmViewer.ViewModel.Lists.Element;
using DsmSuite.DsmViewer.ViewModel.Lists.Relation;
using ReactiveUI;

namespace DsmSuite.DsmViewer.ViewModel.Main
{
    public interface IMainViewModel : INotifyPropertyChanged
    {
        void NotifyElementsReportReady(ElementListViewModelType viewModelType, IDsmElement selectedConsumer, IDsmElement selectedProvider);
        void NotifyRelationsReportReady(RelationsListViewModelType viewModelType, IDsmElement selectedConsumer, IDsmElement selectedProvider);

        IReactiveCommand ToggleElementExpandedCommand { get; }
        IReactiveCommand MoveUpElementCommand { get; }
        IReactiveCommand MoveDownElementCommand { get; }

        IReactiveCommand ToggleElementBookmarkCommand { get; }

        IReactiveCommand SortElementCommand { get; }
        IReactiveCommand ShowElementDetailMatrixCommand { get; }
        IReactiveCommand ShowElementContextMatrixCommand { get; }
        IReactiveCommand ShowCellDetailMatrixCommand { get; }

        IReactiveCommand AddElementCommand { get; }
        IReactiveCommand ModifyElementCommand { get; }
        IReactiveCommand DeleteElementCommand { get; }
        IReactiveCommand ChangeElementParentCommand { get; }

        IndicatorViewMode SelectedIndicatorViewMode { get; }
    }
}
