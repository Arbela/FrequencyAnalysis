using GalaSoft.MvvmLight.CommandWpf;
using Ookii.Dialogs.Wpf;
using System.IO;
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
            dlg.ShowDialog();

            if (string.IsNullOrWhiteSpace(dlg.SelectedPath)) return;

            try
            {
                this.IsBusy = true;

                await Task.Run(() =>
                {
                    this.imageRetriever.RetrieveImages(this.Mp4Path, dlg.SelectedPath, framesPerSec);
                });
            }
            finally
            {
                this.IsBusy = false;
            }
        }
    }
}
