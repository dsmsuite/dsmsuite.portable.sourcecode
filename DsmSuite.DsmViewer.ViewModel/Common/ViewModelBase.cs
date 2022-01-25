using System.ComponentModel;
using System.Runtime.CompilerServices;
using ReactiveUI;

namespace DsmSuite.DsmViewer.ViewModel.Common
{
    public class ReactiveViewModelBase : ReactiveObject
    {
    }

    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
        }
    }
}
