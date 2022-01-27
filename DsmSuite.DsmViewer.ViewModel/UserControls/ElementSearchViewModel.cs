using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ReactiveUI;
using System.Reactive.Linq;

namespace DsmSuite.DsmViewer.ViewModel.Main
{
    public class ElementSearchViewModel : ReactiveViewModelBase
    {
        private readonly IDsmApplication _application;
        private readonly IDsmElement _searchPathElement;
        private IDsmElement _selectedElement;
        private readonly bool _markMatchingElements;

        private string _searchText;
        private bool _caseSensitiveSearch;
        private string _selectedElementType;
        private ObservableCollection<string> _searchMatches;
        private SearchState _searchState;
        private string _searchResult;

        public event EventHandler SearchUpdated;

        public ElementSearchViewModel(IDsmApplication application, IDsmElement searchPathElement, IDsmElement selectedElement, string preSelectedElementType, bool markMatchingElements)
        {
            _application = application;
            _searchPathElement = (searchPathElement != null) ? searchPathElement : _application.RootElement;
            _selectedElement = selectedElement;
            _markMatchingElements = markMatchingElements;

            if ((searchPathElement != null) && (!searchPathElement.HasChildren))
            {
                _selectedElement = searchPathElement;
            }
            if (_selectedElement != null)
            {
                SearchText = _selectedElement.Fullname;
                SearchState = SearchState.Off;
            }
            else
            {
                SearchText = "";
                SearchState = SearchState.NoInput;
            }

            SearchPath = (searchPathElement != null) ? searchPathElement.Fullname : "";

            ElementTypes = new List<string>(application.GetElementTypes());
            SelectedElementType = preSelectedElementType;

            ClearSearchCommand = ReactiveCommand.Create(ClearSearchExecute, ClearSearchCanExecute);
        }

        public List<string> ElementTypes { get; }

        public IReactiveCommand ClearSearchCommand { get; }

        public string SearchPath
        {
            get;
        }

        public IDsmElement SelectedElement
        {
            get { return _selectedElement; }
            set { this.RaiseAndSetIfChanged(ref _selectedElement, value); }
        }

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (_searchText != value)
                {
                    _searchText = this.RaiseAndSetIfChanged(ref _searchText, value);
                    OnSearchTextUpdated();
                }      
            }
        }

        public bool CaseSensitiveSearch
        {
            get { return _caseSensitiveSearch; }
            set 
            {
                if (_caseSensitiveSearch != value)
                {
                    _caseSensitiveSearch = this.RaiseAndSetIfChanged(ref _caseSensitiveSearch, value); ;
                    OnSearchTextUpdated();
                }          
            }
        }

        public string SelectedElementType
        {
            get { return _selectedElementType; }
            set 
            {
                if (_selectedElementType != value)
                {
                    _selectedElementType = this.RaiseAndSetIfChanged(ref _selectedElementType, value); ;
                    OnSearchTextUpdated();
                }           
            }
        }

        public ObservableCollection<string> SearchMatches
        {
            get { return _searchMatches; }
            private set { this.RaiseAndSetIfChanged(ref _searchMatches, value); }
        }

        public SearchState SearchState
        {
            get { return _searchState; }
            set { this.RaiseAndSetIfChanged(ref _searchState, value); }
        }

        public string SearchResult
        {
            get { return _searchResult; }
            set { this.RaiseAndSetIfChanged(ref _searchResult, value); }
        }

        private void OnSearchTextUpdated()
        {
            if (SearchState != SearchState.Off)
            {
                IList<IDsmElement> matchingElements = _application.SearchElements(SearchText, _searchPathElement, CaseSensitiveSearch, SelectedElementType, _markMatchingElements);
                if (SearchText != null)
                {
                    List<string> matchingElementNames = new List<string>();
                    foreach (IDsmElement matchingElement in matchingElements)
                    {
                        matchingElementNames.Add(matchingElement.Fullname);
                    }
                    SearchMatches = new ObservableCollection<string>(matchingElementNames);

                    if (SearchText.Length == 0)
                    {
                        SearchState = SearchState.NoInput;
                        SearchResult = "";
                        SelectedElement = null;
                    }
                    else if (SearchMatches.Count == 0)
                    {
                        SearchState = SearchState.NoMatch;
                        SearchResult = SearchText.Length > 0 ? "None found" : "";
                        SelectedElement = null;
                    }
                    else
                    {
                        SearchState = SearchState.Match;
                        SearchResult = $"{SearchMatches.Count} found";

                        if (SearchMatches.Count == 1)
                        {
                            SearchText = matchingElements[0].Fullname;
                        }
                    }

                    foreach (IDsmElement matchingElement in matchingElements)
                    {
                        if (SearchText == matchingElement.Fullname)
                        {
                            SelectedElement = matchingElement;
                        }
                    }

                    SearchUpdated?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public void ClearSearchExecute()
        {
            SearchText = "";
        }

        private IObservable<bool>? ClearSearchCanExecute
        {
            get
            {
                return Observable.Return(SearchState != SearchState.Off);
            }
        }
    }
}
