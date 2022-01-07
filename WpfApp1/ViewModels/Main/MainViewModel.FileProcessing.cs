using FrequencyAnalysis.Helpers;
using GalaSoft.MvvmLight.CommandWpf;
using Ookii.Dialogs.Wpf;
using System;
using System.Threading.Tasks;

namespace FrequencyAnalysis
{
    public partial class MainViewModel
    {
        private RelayCommand<int> saveFramesCommand;


        public RelayCommand<int> SaveFramesCommand
        {
            get => this.saveFramesCommand ?? (this.saveFramesCommand = new RelayCommand<int>(SaveFramesCommandExecuted));
        }

        private async void SaveFramesCommandExecuted(int framesPerSec)
        {
            var dlg = new VistaFolderBrowserDialog();
            dlg.ShowNewFolderButton = true;
            dlg.ShowDialog();

            if (string.IsNullOrWhiteSpace(dlg.SelectedPath)) return;

            try
            {
                this.IsBusy = true;

                await Task.Run(() =>
                {
                    if (!TimeSpan.TryParseExact(this.VideoProcessingEnd, new string[] { "mm\\:ss", "mmss", "hhmmss", "hh\\:mm\\:ss" }, null, out TimeSpan to) ||
                    to > this.Mp4.Metadata.Duration)
                    {
                        to = this.Mp4.Metadata.Duration;
                    }

                    if (!TimeSpan.TryParseExact(this.VideoProcessingStart, new string[] { "mm\\:ss", "mmss", "hhmmss", "hh\\:mm\\:ss" }, null, out TimeSpan from) ||
                    from > to)
                    {
                        from = TimeSpan.Zero;
                    }  
                    
                    this.imageRetriever.RetrieveImages(this.Mp4Path, dlg.SelectedPath, framesPerSec, from, to);
                });
            }
            finally
            {
                this.IsBusy = false;
            }
        }
    }
}
