using DsmSuite.DsmViewer.ViewModel.Common;

namespace DsmSuite.DsmViewer.ViewModel.Matrix
{
    public class LegendItemViewModel : ReactiveViewModelBase
    {
        public LegendItemViewModel(LegendColor color, string description)
        {
            Color = color;
            Description = description;
        }

        public LegendColor Color { get; }
        public string Description { get; }
    }
}
