using FrequencyAnalysis.Helpers;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace FrequencyAnalysis
{
    public partial class MainViewModel
    {
        private bool isVideoUploaded;
        private ICommand openVideoCommand;
        private ICommand closeVideoCommand;
        private RelayCommand<object> playVideoCommand;
        private RelayCommand<object> stopVideoCommand;

        public bool IsVideoUploaded
        {
            get => isVideoUploaded;
            set
            {
                isVideoUploaded = value;
                RaisePropertyChanged(nameof(IsVideoUploaded));
            }
        }

        public ICommand OpenVideoCommand
        {
            get => this.openVideoCommand ?? (this.openVideoCommand = new RelayCommand(OpenVideoCommandExecuted, () => this.SelectedImagePath == null));
        }

        public ICommand CloseVideoCommand
        {
            get => this.closeVideoCommand ?? (this.closeVideoCommand = new RelayCommand(CloseVideoCommandExecuted));
        }

        public RelayCommand<object> PlayVideoCommand
        {
            get => this.playVideoCommand ?? (this.playVideoCommand = new RelayCommand<object>(PlayVideoCommandExecuted, (obj) => this.IsVideoUploaded));
        }

        public RelayCommand<object> StopVideoCommand
        {
            get => this.stopVideoCommand ?? (this.stopVideoCommand = new RelayCommand<object>(StopVideoCommandExecuted, (obj) => this.IsVideoUploaded));
        }

        private void StopVideoCommandExecuted(object obj)
        {
            var mediaElement = obj as MediaElement;

            if (mediaElement == null) return;

            mediaElement.Stop();
        }

        private void PlayVideoCommandExecuted(object obj)
        {
            var mediaElement = obj as MediaElement;

            if (mediaElement == null) return;

            mediaElement.Play();
        }

        private void CloseVideoCommandExecuted()
        {
            this.Mp4Path = null;
            this.IsVideoUploaded = false;
        }

        private void OpenVideoCommandExecuted()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            fileDialog.Multiselect = false;
            fileDialog.Filter = Constants.Mp4Filter;
            fileDialog.ShowDialog();

            if (fileDialog.FileNames.Any())
            {
                this.Mp4Path = fileDialog.FileName;
                this.IsVideoUploaded = true;
            }
        }
    }
}
