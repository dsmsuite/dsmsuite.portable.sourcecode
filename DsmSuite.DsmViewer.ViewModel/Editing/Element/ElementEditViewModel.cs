using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;
using ReactiveUI;

namespace DsmSuite.DsmViewer.ViewModel.Editing.Element
{
    public class ElementEditViewModel : ReactiveViewModelBase
    {
        private readonly ElementEditViewModelType _viewModelType;
        private readonly IDsmApplication _application;
        private readonly IDsmElement _parentElement;
        private readonly IDsmElement _selectedElement;
        private string _name;
        private string _help;
        private string _selectedElementType;

        private static string _lastSelectedElementType = "";

        public IReactiveCommand AcceptChangeCommand { get; }

        public ElementEditViewModel(ElementEditViewModelType viewModelType, IDsmApplication application, IDsmElement selectedElement)
        {
            _viewModelType = viewModelType;
            _application = application;

            ElementTypes = new List<string>(application.GetElementTypes());

            switch (_viewModelType)
            {
                case ElementEditViewModelType.Modify:
                    Title = "Modify element";
                    _parentElement = selectedElement.Parent;
                    _selectedElement = selectedElement;
                    Name = _selectedElement.Name;
                    SelectedElementType = _selectedElement.Type;
                    AcceptChangeCommand = ReactiveCommand.Create(AcceptModifyExecute, AcceptCanExecute);
                    break;
                case ElementEditViewModelType.Add:
                    Title = "Add element";
                    _parentElement = selectedElement;
                    _selectedElement = null;
                    Name = "";
                    SelectedElementType = _lastSelectedElementType;
                    AcceptChangeCommand = ReactiveCommand.Create(AcceptAddExecute, AcceptCanExecute);
                    break;
                default:
                    break;
            }
        }

        public string Title { get; }

        public string Help
        {
            get { return _help; }
            private set { this.RaiseAndSetIfChanged(ref _help, value); }
        }

        public string Name
        {
            get { return _name; }
            set { this.RaiseAndSetIfChanged(ref _name, value); }
        }

        public List<string> ElementTypes { get; }

        public string SelectedElementType
        {
            get { return _selectedElementType; }
            set { _lastSelectedElementType = this.RaiseAndSetIfChanged(ref _selectedElementType, value); }
        }

        private void AcceptAddExecute()
        {
            _application.CreateElement(Name, SelectedElementType, _parentElement);
        }

        private void AcceptModifyExecute()
        {
            if (_selectedElement.Name != Name)
            {
                _application.ChangeElementName(_selectedElement, Name);
            }

            if (_selectedElement.Type != SelectedElementType)
            {
                _application.ChangeElementType(_selectedElement, SelectedElementType);
            }
        }

        private IObservable<bool>? AcceptCanExecute
        {
            get
            {
                bool canExecute = false;
                ElementName elementName = new ElementName(_parentElement.Fullname);
                elementName.AddNamePart(Name);
                IDsmElement existingElement = _application.GetElementByFullname(elementName.FullName);

                if (Name.Length == 0)
                {
                    Help = "Name can not be empty";
                }
                else if (Name.Contains("."))
                {
                    Help = "Name can not be contain dot character";
                }
                else if ((existingElement != _selectedElement) && (existingElement != null))
                {
                    Help = "Name can not be an existing name";
                }
                else
                {
                    Help = "";
                    canExecute = true;
                }

                return Observable.Return(canExecute);
            }
        }
    }
}
