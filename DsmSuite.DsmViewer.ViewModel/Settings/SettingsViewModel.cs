using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;
using ReactiveUI;

namespace DsmSuite.DsmViewer.ViewModel.Settings
{
    public class SettingsViewModel : ReactiveViewModelBase
    {
        private const string PastelThemeName = "Pastel";
        private const string LightThemeName = "Light";

        private readonly IDsmApplication _application;
        private LogLevel _logLevel;
        private string _selectedThemeName;

        private readonly Dictionary<Theme, string> _supportedThemes;

        public SettingsViewModel(IDsmApplication application)
        {
            _application = application;
            _supportedThemes = new Dictionary<Theme, string>
            {
                [Theme.Pastel] = PastelThemeName,
                [Theme.Light] = LightThemeName
            };

            LogLevel = ViewerSetting.LogLevel;
            SelectedThemeName = _supportedThemes[ViewerSetting.Theme];

            AcceptChangeCommand = ReactiveCommand.Create(AcceptChangeExecute, AcceptChangeCanExecute);
        }

        public IReactiveCommand AcceptChangeCommand { get; }

        public LogLevel LogLevel
        {
            get { return _logLevel; }
            set { this.RaiseAndSetIfChanged(ref _logLevel, value); }
        }

        public List<string> SupportedThemeNames => _supportedThemes.Values.ToList();

        public string SelectedThemeName
        {
            get { return _selectedThemeName; }
            set { this.RaiseAndSetIfChanged(ref _selectedThemeName, value); }
        }

        private void AcceptChangeExecute()
        {
            ViewerSetting.LogLevel = LogLevel;
            ViewerSetting.Theme = _supportedThemes.FirstOrDefault(x => x.Value == SelectedThemeName).Key;
        }

        private IObservable<bool>? AcceptChangeCanExecute
        {
            get
            {
                return Observable.Return(true);
            }
        }
    }
}
