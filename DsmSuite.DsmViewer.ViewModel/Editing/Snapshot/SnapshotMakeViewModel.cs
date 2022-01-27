using System;
using System.Reactive.Linq;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;
using ReactiveUI;

namespace DsmSuite.DsmViewer.ViewModel.Editing.Snapshot
{
    public class SnapshotMakeViewModel : ReactiveViewModelBase
    {
        private readonly IDsmApplication _application;
        private string _description;
        private string _help;

        public IReactiveCommand AcceptChangeCommand { get; }

        public SnapshotMakeViewModel(IDsmApplication application)
        {
            _application = application;

            Title = "Make snapshot";
            Help = "";

            Description = "";
            AcceptChangeCommand = ReactiveCommand.Create(AcceptChangeExecute, AcceptChangeCanExecute);
        }

        public string Title { get; }

        public string Help
        {
            get { return _help; }
            private set { this.RaiseAndSetIfChanged(ref _help, value); }
        }

        public string Description
        {
            get { return _description; }
            set { this.RaiseAndSetIfChanged(ref _description, value); }
        }

        private void AcceptChangeExecute()
        {
            _application.MakeSnapshot(Description);
        }

        private IObservable<bool>? AcceptChangeCanExecute
        {
            get
            {
                return Observable.Return(Description.Length > 0);
            }
        }
    }
}
