using System.ComponentModel;
using System.Runtime.CompilerServices;
using ReactiveUI;

namespace DsmSuite.DsmViewer.ViewModel.Common
{
    public class ViewModelBase : ReactiveObject
    {
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
        }
    }
}
