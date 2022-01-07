using FrequencyAnalysis.Helpers;
using GalaSoft.MvvmLight.CommandWpf;
using MediaToolkit;
using MediaToolkit.Model;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows.Input;

namespace FrequencyAnalysis
{
    public partial class MainViewModel
    {
        private bool isVideoUploaded;
        private string videoProcessingStart;
        private string videoProcessingEnd;
        private ICommand openVideoCommand;
        private ICommand closeVideoCommand;

        public bool IsVideoUploaded
        {
            get => this.isVideoUploaded;
            set
            {
                this.isVideoUploaded = value;
                RaisePropertyChanged(nameof(IsVideoUploaded));
            }
        }

        public string VideoProcessingStart
        {
            get => this.videoProcessingStart;
            set
            {
                this.videoProcessingStart = value;
                RaisePropertyChanged(nameof(VideoProcessingStart));
            }
        }

        public string VideoProcessingEnd
        {
            get => this.videoProcessingEnd;
            set
            {
                this.videoProcessingEnd = value;
                RaisePropertyChanged(nameof(VideoProcessingEnd));
            }
        }

        public ICommand OpenVideoCommand
        {
            get => this.openVideoCommand ?? (this.openVideoCommand = new RelayCommand(OpenVideoCommandExecuted));
        }

        public ICommand CloseVideoCommand
        {
            get => this.closeVideoCommand ?? (this.closeVideoCommand = new RelayCommand(CloseVideoCommandExecuted));
        }

        private void CloseVideoCommandExecuted()
        {
            this.Mp4Path = null;
            this.Mp4 = null;
            this.VideoProcessingStart = null;
            this.VideoProcessingEnd = null;
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
                using (var engine = new Engine())
                {
                    this.Mp4 = new MediaFile { Filename = this.Mp4Path };
                    engine.GetMetadata(Mp4);
                }
                this.VideoProcessingStart = TimeSpan.Zero.ToString("mm\\:ss");
                this.VideoProcessingEnd = Mp4.Metadata.Duration.ToString("mm\\:ss");
                this.IsVideoUploaded = true;
            }
        }
    }
}
