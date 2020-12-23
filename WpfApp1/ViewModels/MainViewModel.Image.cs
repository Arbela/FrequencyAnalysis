using FrequencyAnalysis.Helpers;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FrequencyAnalysis
{
    public partial class MainViewModel
    {
        private ICommand openCommand;
        private ICommand closeCommand;
        private ICommand convertToGrayscaleCommand;
        private ICommand saveCommand;
        private ICommand saveAsCommand;
        private ICommand saveAsTxtCommand;

        public ICommand OpenCommand
        {
            get => this.openCommand ?? (this.openCommand = new RelayCommand(ImageOpenCommandExecuted, () => !this.IsVideoUploaded));
        }

        public ICommand CloseCommand
        {
            get => this.closeCommand ?? (this.closeCommand = new RelayCommand(ImageCloseCommandExecuted, CanExecuteSelectedImagePathRelatedCommand));
        }

        public ICommand ConvertToGrayscaleCommand
        {
            get => this.convertToGrayscaleCommand ?? (this.convertToGrayscaleCommand = new RelayCommand(ConvertToGrayscaleCommandExecuted, CanExecuteSelectedImagePathRelatedCommand));
        }

        public ICommand SaveCommand
        {
            get => this.saveCommand ?? (this.saveCommand = new RelayCommand(SaveCommandExecuted, CanExecuteSelectedImagePathRelatedCommand));
        }

        public ICommand SaveAsCommand
        {
            get => this.saveAsCommand ?? (this.saveAsCommand = new RelayCommand(SaveAsCommandExecuted, CanExecuteSelectedImagePathRelatedCommand));
        }

        public ICommand SaveAsTxtCommand
        {
            get => this.saveAsTxtCommand ?? (this.saveAsTxtCommand = new RelayCommand(SaveAsTxtCommandExecuted, CanExecuteSelectedImagePathRelatedCommand));
        }

        private async void SaveAsTxtCommandExecuted()
        {
            var matrix = this.imageProvider.GetBitmapPixelsMatrix(this.SelectedImagePath);

            var directoryDialog = ShowSaveFileDialog(Constants.TxtFilter, Constants.TxtExtPattern);

            if (!string.IsNullOrEmpty(directoryDialog.FileName))
            {
                using (FileStream fs = File.Create(directoryDialog.FileName))
                {
                    await imageProvider.ExportBitmapPixelsMatrixAsync(this.SelectedImagePath, fs);
                }

                ExportMatrixParameters(directoryDialog.FileName, matrix);
            }
        }

        private void SaveAsCommandExecuted()
        {
            var saveDialog = ShowSaveFileDialog(Constants.ImageFilter, this.selectedImageFormat);

            if (!string.IsNullOrWhiteSpace(saveDialog.FileName))
            {
                Image.FromFile(this.SelectedImagePath).Save(saveDialog.FileName);
            }
        }

        private void SaveCommandExecuted()
        {
            var saveDialog = ShowSaveFileDialog(defaultExt: this.selectedImageFormat);

            if (!string.IsNullOrWhiteSpace(saveDialog.FileName))
            {
                Image.FromFile(this.SelectedImagePath).Save(saveDialog.FileName);
            }
        }

        private async void ConvertToGrayscaleCommandExecuted()
        {
            await Task.Run( async () =>
            {
                string bitmapPath = CreateLocalFile($"{Constants.BmpImageName}{Constants.BmpExt}");

                await this.imageProvider.CreateGrayscale8Async(this.SelectedImagePath, bitmapPath);

                this.PixelsMatrix = this.imageProvider.GetBitmapPixelsMatrix(bitmapPath);
                this.GradientMatrix = this.gradientMatrixBuilder.BuildGradientMatrix(this.PixelsMatrix);
                this.LinearContrastMatrix = this.linearContraster.BuildLinearContrastMatrix(this.GradientMatrix);

                this.SelectedImagePath = bitmapPath;
            });
        }

        private void ImageOpenCommandExecuted()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            fileDialog.Multiselect = false;
            fileDialog.Filter = Constants.ImageFilter;
            fileDialog.ShowDialog();

            if (fileDialog.FileNames.Any())
            {
                this.SelectedImagePath = fileDialog.FileNames[0];
            }
        }

        private void ImageCloseCommandExecuted() => this.SelectedImagePath = null;

        private bool CanExecuteSelectedImagePathRelatedCommand() => !string.IsNullOrWhiteSpace(this.SelectedImagePath);
    }
}
