using FrequencyAnalysis.Helpers;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FrequencyAnalysis
{
    public partial class MainViewModel
    {
        private ICommand imageFileMagnitude;
        private ICommand imageFileVertical;
        private ICommand imageFileHorizontal;
        private ICommand textFileMagnitude;
        private ICommand textFileVertical;
        private ICommand textFileHorizontal;

        public ICommand ImageFileMagnitude
        {
            get => this.imageFileMagnitude ?? (this.imageFileMagnitude = new RelayCommand(ImageFileMagnitudeExecuted, CanExecuteSelectedImagePathRelatedCommand));
        }

        public ICommand ImageFileVertical
        {
            get => this.imageFileVertical ?? (this.imageFileVertical = new RelayCommand(ImageFileVerticalExecuted, CanExecuteSelectedImagePathRelatedCommand));
        }

        public ICommand ImageFileHorizontal
        {
            get => this.imageFileHorizontal ?? (this.imageFileHorizontal = new RelayCommand(ImageFileHorizontalExecuted, CanExecuteSelectedImagePathRelatedCommand));
        }

        public ICommand TextFileMagnitude
        {
            get => this.textFileMagnitude ?? (this.textFileMagnitude = new RelayCommand(TextFileMagnitudeExecuted, CanExecuteSelectedImagePathRelatedCommand));
        }

        public ICommand TextFileVertical
        {
            get => this.textFileVertical ?? (this.textFileVertical = new RelayCommand(TextFileVerticalExecuted, CanExecuteSelectedImagePathRelatedCommand));
        }

        public ICommand TextFileHorizontal
        {
            get => this.textFileHorizontal ?? (this.textFileHorizontal = new RelayCommand(TextFileHorizontalExecuted, CanExecuteSelectedImagePathRelatedCommand));
        }

        private async void TextFileHorizontalExecuted()
        {
            await SaveToTxtFile(horizontalOnly: true, verticalOnly: false);
        }

        private async void TextFileVerticalExecuted()
        {
            await SaveToTxtFile(horizontalOnly: false, verticalOnly: true);
        }

        private async void TextFileMagnitudeExecuted()
        {
            await SaveToTxtFile(horizontalOnly: false, verticalOnly: false);
        }

        private async void ImageFileHorizontalExecuted()
        {
            await SaveImageFile(verticalOnly: false, horizontalOnly: true);
        }

        private async void ImageFileVerticalExecuted()
        {
            await SaveImageFile(verticalOnly: true, horizontalOnly: false);
        }

        private async void ImageFileMagnitudeExecuted()
        {
            await SaveImageFile(verticalOnly: false, horizontalOnly: false);
        }

        private async Task SaveToTxtFile(bool verticalOnly, bool horizontalOnly)
        {
            var directoryDialog = ShowSaveFileDialog(Constants.TxtFilter, Constants.TxtExtPattern);

            if (string.IsNullOrEmpty(directoryDialog.FileName)) return;

            string bitmapPath = CreateLocalFile($"{Constants.BmpImageName}{Constants.BmpExt}");

            var pixelsMatrix = this.imageProvider.GetBitmapPixelsMatrix(bitmapPath);
            var gradientMatrix = this.gradientMatrixBuilder.BuildGradientMatrix(pixelsMatrix, verticalOnly, horizontalOnly);
            var linearMatrix = this.linearContraster.BuildLinearContrastMatrix(gradientMatrix);

            await ExportMatrix(directoryDialog.FileName, linearMatrix);

            ExportMatrixParameters(directoryDialog.FileName, linearMatrix);
        }

        private async Task SaveImageFile(bool verticalOnly, bool horizontalOnly)
        {
            var saveDialog = ShowSaveFileDialog(defaultExt: this.selectedImageFormat);

            if (string.IsNullOrEmpty(saveDialog.FileName)) return;

            string bitmapPath = CreateLocalFile($"{Constants.BmpImageName}{Constants.BmpExt}");

            int[][] gradientMatrix = new int[0][];

            await Task.Run(async () =>
            {
                await this.imageProvider.CreateGrayscale8Async(this.SelectedImagePath, bitmapPath);

                var pixelsMatrix = this.imageProvider.GetBitmapPixelsMatrix(bitmapPath);
                gradientMatrix = this.gradientMatrixBuilder.BuildGradientMatrix(pixelsMatrix, verticalOnly, horizontalOnly);
            });

            this.imageProvider.CreateBitmapFromPixelMartix(gradientMatrix).Save(saveDialog.FileName);
        }
    }
}
