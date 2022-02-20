using System.Collections.Generic;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;
using System.Text;
using ReactiveUI;

namespace DsmSuite.DsmViewer.ViewModel.Lists.Action
{
    public class ActionListViewModel : ReactiveViewModelBase
    {
        private readonly IDsmApplication _application;
        private IEnumerable<ActionListItemViewModel> _actions;

        public ActionListViewModel(IDsmApplication application)
        {
            Title = "Edit history";

            _application = application;
            _application.ActionPerformed += OnActionPerformed;

            UpdateActionList();
            
            CopyToClipboardCommand =  ReactiveCommand.Create(CopyToClipboardExecute);
            ClearCommand = ReactiveCommand.Create(ClearExecute);
        }

        private void OnActionPerformed(object sender, System.EventArgs e)
        {
            UpdateActionList();
        }

        public string Title { get; }
        public string SubTitle { get; }

        public IEnumerable<ActionListItemViewModel> Actions
        {
            get { return _actions; }
            set { this.RaiseAndSetIfChanged(ref _actions, value); }
        }

        public IReactiveCommand CopyToClipboardCommand { get; }
        public IReactiveCommand ClearCommand { get; }

        private void CopyToClipboardExecute()
        {
            StringBuilder builder = new StringBuilder();
            foreach(ActionListItemViewModel viewModel in Actions)
            {
                builder.AppendLine($"{viewModel.Index, -5}, {viewModel.Action, -30}, {viewModel.Details}");
            }
            // TODO: Fix Clipboard.SetText(builder.ToString());
        }

        private void ClearExecute()
        {
            _application.ClearActions();
            UpdateActionList();
        }

        private void UpdateActionList()
        {
            var actionViewModels = new List<ActionListItemViewModel>();
            int index = 1;
            foreach (IAction action in _application.GetActions())
            {
                actionViewModels.Add(new ActionListItemViewModel(index, action));
                index++;
            }

            Actions = actionViewModels;
        }
    }
}
