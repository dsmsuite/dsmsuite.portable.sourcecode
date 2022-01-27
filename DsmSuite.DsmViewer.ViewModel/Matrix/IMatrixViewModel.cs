using ReactiveUI;

namespace DsmSuite.DsmViewer.ViewModel.Matrix
{
    public interface IMatrixViewModel
    {
        IReactiveCommand ToggleElementExpandedCommand { get; }
        IReactiveCommand SortElementCommand { get; }
        IReactiveCommand MoveUpElementCommand { get; }
        IReactiveCommand MoveDownElementCommand { get; }

        IReactiveCommand ToggleElementBookmarkCommand { get; }

        IReactiveCommand ChangeElementParentCommand { get; }
    }
}
