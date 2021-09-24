using FrequencyAnalysis.Helpers;
using GalaSoft.MvvmLight.CommandWpf;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace FrequencyAnalysis
{
    public partial class MainViewModel
    {
        private RelayCommand<int> blurGeneralCommad;
        private RelayCommand convertToGrayscaleCommand;


        public RelayCommand<int> BlurGeneralCommand
        {
            get => this.blurGeneralCommad ?? (this.blurGeneralCommad = new RelayCommand<int>(BlurGeneralCommandExecuted));
        }

        public RelayCommand ConvertToGrayscaleCommand
        {
            get => this.convertToGrayscaleCommand ?? (this.convertToGrayscaleCommand = new RelayCommand(ConvertToGrayscaleCommandExecuted, CanExecuteSelectedImagePathRelatedCommand));
        }

        private async void ConvertToGrayscaleCommandExecuted()
        {
            var directoryDialog = ShowSaveFileDialog(Constants.BmpFilter, Constants.BmpExtPattern);

            if (string.IsNullOrEmpty(directoryDialog.FileName)) return;

            try
            {
                this.IsBusy = true;

                string bitmapPath = directoryDialog.FileName;
                await this.imageProvider.CreateGrayscale8Async(this.SelectedImagePath, bitmapPath);
                this.SelectedImagePath = bitmapPath;
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private bool CanExecuteBlurGeneralCommand(int arg)
        {
            return !string.IsNullOrWhiteSpace(this.SelectedImagePath);
        }

        private async void BlurGeneralCommandExecuted(int blockSize)
        {
            if (blockSize == 3)
            {
                var directoryDialog = ShowSaveFileDialog(Constants.BmpFilter, Constants.BmpExtPattern);

                if (string.IsNullOrEmpty(directoryDialog.FileName)) return;

                try
                {
                    this.IsBusy = true;

                    string bitmapPath = directoryDialog.FileName;

                    await Task.Run(() =>
                    {
                        this.PixelsMatrix = this.imageProvider.GetBitmapPixelsMatrix(this.SelectedImagePath);
                        this.GradientMatrix = this.gradientMatrixBuilder.BuildGradientMatrix(this.PixelsMatrix);
                        this.LinearContrastMatrix = this.linearContraster.BuildLinearContrastMatrix(this.GradientMatrix);

                        var blurBitmap = this.imageProvider.CreateBitmapFromPixelMartix(this.LinearContrastMatrix);
                        blurBitmap.Save(bitmapPath, ImageFormat.Bmp);
                    });

                    this.SelectedImagePath = bitmapPath;
                }
                finally
                {
                    this.IsBusy = false;
                }
            }
        }

    }
}
