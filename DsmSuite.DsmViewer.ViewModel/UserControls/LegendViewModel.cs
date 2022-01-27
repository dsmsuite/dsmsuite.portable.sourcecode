using DsmSuite.DsmViewer.ViewModel.Common;
using DsmSuite.DsmViewer.ViewModel.Main;
using ReactiveUI;

namespace DsmSuite.DsmViewer.ViewModel.UserControls
{
    public class LegendViewModel : ReactiveViewModelBase
    {
        private IndicatorViewMode _selectedIndicatorViewMode;

        public IndicatorViewMode SelectedIndicatorViewMode
        {
            get { return _selectedIndicatorViewMode; }
            set { this.RaiseAndSetIfChanged(ref _selectedIndicatorViewMode, value); }
        }
    }
}
