using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;

namespace DsmSuite.DsmViewer.ViewModel.Matrix
{
    public class ElementToolTipViewModel : ReactiveViewModelBase
    {
        public ElementToolTipViewModel(IDsmElement element, IDsmApplication application)
        {
            Title = $"Element {element.Name}";
            Id = element.Id;
            Name = element.Fullname;
            Type = element.Type;
        }

        public string Title { get; }
        public int Id { get; }
        public string Name { get; }
        public string Type { get; }
    }
}
