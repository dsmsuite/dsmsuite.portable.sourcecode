using System;
using DsmSuite.DsmViewer.ViewModel.Common;
using DsmSuite.Common.Util;
using ReactiveUI;

namespace DsmSuite.DsmViewer.ViewModel.Common
{
    public class ProgressViewModel : ReactiveViewModelBase
    {
        public event EventHandler<bool> BusyChanged;

        private bool _busy;
        private string _action;
        private string _text;
        private int _progressValue;
        private string _progressText;

        public void Update(ProgressInfo progress)
        {
            Text = progress.ActionText;
            if (progress.Percentage.HasValue)
            {
                ProgressText = $"{progress.CurrentItemCount}/{progress.TotalItemCount} {progress.ItemType}";
                ProgressValue = progress.Percentage.Value;
            }
            Busy = !progress.Done;
        }

        public string Action
        {
            get { return _action; }
            set { this.RaiseAndSetIfChanged(ref _action, value); }
        }

        public string Text
        {
            get { return _text; }
            set { this.RaiseAndSetIfChanged(ref _text, value); }
        }

        public bool Busy
        {
            get { return _busy; }
            set
            {
                if (_busy != value)
                {
                    this.RaiseAndSetIfChanged(ref _busy, value);
                    BusyChanged?.Invoke(this, _busy);
                }
            }
        }

        public int ProgressValue
        {
            get { return _progressValue; }
            set { this.RaiseAndSetIfChanged(ref _progressValue, value); }
        }

        public string ProgressText
        {
            get { return _progressText; }
            set { this.RaiseAndSetIfChanged(ref _progressText, value); }
        }
    }
}
