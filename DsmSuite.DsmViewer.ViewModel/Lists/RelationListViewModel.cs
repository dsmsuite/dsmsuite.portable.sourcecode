using System.Collections.Generic;
using DsmSuite.DsmViewer.ViewModel.Common;
using System.Text;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.Application.Interfaces;
using System.Collections.ObjectModel;
using DsmSuite.DsmViewer.ViewModel.Editing.Relation;
using System;
using ReactiveUI;
using System.Reactive.Linq;

namespace DsmSuite.DsmViewer.ViewModel.Lists
{
    public class RelationListViewModel : ReactiveViewModelBase
    {
        private readonly RelationsListViewModelType _viewModelType;
        private readonly IDsmApplication _application;
        private readonly IDsmElement _selectedConsumer;
        private readonly IDsmElement _selectedProvider;
        private ObservableCollection<RelationListItemViewModel> _relations;
        private RelationListItemViewModel _selectedRelation;

        public event EventHandler<RelationEditViewModel> RelationAddStarted;
        public event EventHandler<RelationEditViewModel> RelationEditStarted;

        public RelationListViewModel(RelationsListViewModelType viewModelType, IDsmApplication application, IDsmElement selectedConsumer, IDsmElement selectedProvider)
        {
            _viewModelType = viewModelType;
            _application = application;
            _selectedConsumer = selectedConsumer;
            _selectedProvider = selectedProvider;

            Title = "Relation List";
            switch (viewModelType)
            {
                case RelationsListViewModelType.ElementIngoingRelations:
                    SubTitle = $"Ingoing relations of {_selectedProvider.Fullname}";
                    AddRelationCommand = ReactiveCommand.Create(AddConsumerRelationExecute, AddRelationCanExecute);
                    break;
                case RelationsListViewModelType.ElementOutgoingRelations:
                    SubTitle = $"Outgoing relations of {_selectedProvider.Fullname}";
                    AddRelationCommand = ReactiveCommand.Create(AddProviderRelationExecute, AddRelationCanExecute);
                    break;
                case RelationsListViewModelType.ElementInternalRelations:
                    SubTitle = $"Internal relations of {_selectedProvider.Fullname}";
                    AddRelationCommand = ReactiveCommand.Create(AddInternalRelationExecute, AddRelationCanExecute);
                    break;
                case RelationsListViewModelType.ConsumerProviderRelations:
                    SubTitle = $"Relations between consumer {_selectedConsumer.Fullname} and provider {_selectedProvider.Fullname}";
                    AddRelationCommand = ReactiveCommand.Create(AddConsumerProviderRelationExecute, AddRelationCanExecute);
                    break;
                default:
                    SubTitle = "";
                    break;
            }

            CopyToClipboardCommand = ReactiveCommand.Create(CopyToClipboardExecute);
            DeleteRelationCommand = ReactiveCommand.Create(DeleteRelationExecute, DeleteRelationCanExecute);
            EditRelationCommand = ReactiveCommand.Create(EditRelationExecute, EditRelationCanExecute);

            UpdateRelations(null);
        }

        public string Title { get; }
        public string SubTitle { get; }

        public ObservableCollection<RelationListItemViewModel> Relations
        {
            get { return _relations; }
            private set { this.RaiseAndSetIfChanged(ref _relations, value); }
        }

        public RelationListItemViewModel SelectedRelation
        {
            get { return _selectedRelation; }
            set { this.RaiseAndSetIfChanged(ref _selectedRelation, value); }
        }

        public IReactiveCommand CopyToClipboardCommand { get; }
        public IReactiveCommand DeleteRelationCommand { get; }
        public IReactiveCommand EditRelationCommand { get; }
        public IReactiveCommand AddRelationCommand { get; }

        private void CopyToClipboardExecute()
        {
            StringBuilder builder = new StringBuilder();
            foreach (RelationListItemViewModel viewModel in Relations)
            {
                builder.AppendLine($"{viewModel.Index,-5}, {viewModel.ConsumerName,-100}, {viewModel.ProviderName,-100}, {viewModel.RelationType,-30}, {viewModel.RelationWeight,-10}, {viewModel.Properties,-150}");
            }
            // TODO: Fix Clipboard.SetText(builder.ToString());
        }

        private void DeleteRelationExecute()
        {
            _application.DeleteRelation(SelectedRelation.Relation);
            UpdateRelations(SelectedRelation.Relation);
        }

        private IObservable<bool>? DeleteRelationCanExecute
        {
            get
            {
                return Observable.Return(SelectedRelation != null);
            }
        }

        private void EditRelationExecute()
        {
            RelationEditViewModel relationEditViewModel = new RelationEditViewModel(RelationEditViewModelType.Modify, _application, SelectedRelation.Relation, null, null, null, null);
            relationEditViewModel.RelationUpdated += OnRelationUpdated;
            RelationEditStarted?.Invoke(this, relationEditViewModel);
        }

        private IObservable<bool>? EditRelationCanExecute
        {
            get
            {
                return Observable.Return(SelectedRelation != null);
            }
        }

        private void AddConsumerRelationExecute()
        {
            RelationEditViewModel relationEditViewModel = new RelationEditViewModel(RelationEditViewModelType.Add, _application, null, _application.RootElement, null, null, _selectedProvider);
            relationEditViewModel.RelationUpdated += OnRelationUpdated;
            RelationAddStarted?.Invoke(this, relationEditViewModel);
        }

        private void AddProviderRelationExecute()
        {
            RelationEditViewModel relationEditViewModel = new RelationEditViewModel(RelationEditViewModelType.Add, _application, null, null, _selectedProvider, _application.RootElement, null);
            relationEditViewModel.RelationUpdated += OnRelationUpdated;
            RelationAddStarted?.Invoke(this, relationEditViewModel);
        }

        private void AddInternalRelationExecute()
        {
            RelationEditViewModel relationEditViewModel = new RelationEditViewModel(RelationEditViewModelType.Add, _application, null, _selectedProvider, null, _selectedProvider, null);
            relationEditViewModel.RelationUpdated += OnRelationUpdated;
            RelationAddStarted?.Invoke(this, relationEditViewModel);
        }

        private void AddConsumerProviderRelationExecute()
        {
            RelationEditViewModel relationEditViewModel = new RelationEditViewModel(RelationEditViewModelType.Add, _application, null, _selectedConsumer, null, _selectedProvider, null);
            relationEditViewModel.RelationUpdated += OnRelationUpdated;
            RelationAddStarted?.Invoke(this, relationEditViewModel);
        }

        private IObservable<bool>? AddRelationCanExecute
        {
            get
            {
                return Observable.Return(true);
            }
        }

        private void OnRelationUpdated(object sender, IDsmRelation updatedRelation)
        {
            UpdateRelations(updatedRelation);
        }

        private void UpdateRelations(IDsmRelation updatedRelation)
        {
            RelationListItemViewModel selectedRelationListItemViewModel = null;
            IEnumerable<IDsmRelation> relations;
            switch (_viewModelType)
            {
                case RelationsListViewModelType.ElementIngoingRelations:
                    relations = _application.FindIngoingRelations(_selectedProvider);
                    break;
                case RelationsListViewModelType.ElementOutgoingRelations:
                    relations = _application.FindOutgoingRelations(_selectedProvider);
                    break;
                case RelationsListViewModelType.ElementInternalRelations:
                    relations = _application.FindInternalRelations(_selectedProvider);
                    break;
                case RelationsListViewModelType.ConsumerProviderRelations:
                    relations = _application.FindResolvedRelations(_selectedConsumer, _selectedProvider);
                    break;
                default:
                    relations = new List<IDsmRelation>();
                    break;
            }

            List<RelationListItemViewModel> relationViewModels = new List<RelationListItemViewModel>();

            foreach (IDsmRelation relation in relations)
            {
                RelationListItemViewModel relationListItemViewModel = new RelationListItemViewModel(_application, relation);
                relationViewModels.Add(relationListItemViewModel);

                if (updatedRelation != null)
                {
                    if (relation.Id == updatedRelation.Id)
                    {
                        selectedRelationListItemViewModel = relationListItemViewModel;
                    }
                }
            }

            relationViewModels.Sort();

            int index = 1;
            foreach (RelationListItemViewModel viewModel in relationViewModels)
            {
                viewModel.Index = index;
                index++;
            }

            Relations = new ObservableCollection<RelationListItemViewModel>(relationViewModels);
            SelectedRelation = selectedRelationListItemViewModel;
        }
    }
}
