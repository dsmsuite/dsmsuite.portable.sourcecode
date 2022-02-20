using DsmSuite.DsmViewer.ViewModel.Common;
using ReactiveUI;

namespace DsmSuite.DsmViewer.ViewModel.Legend
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
