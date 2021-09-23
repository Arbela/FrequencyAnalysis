using FrequencyAnalysis.Helpers;
using GalaSoft.MvvmLight.CommandWpf;

namespace FrequencyAnalysis
{
    public partial class MainViewModel
    {
        private RelayCommand<int> blurGeneralCommad;

        public RelayCommand<int> BlurGeneralCommand
        {
            get => this.blurGeneralCommad ?? (this.blurGeneralCommad = new RelayCommand<int>(BlurGeneralCommandExecuted));
        }

        private bool CanExecuteBlurGeneralCommand(int arg)
        {
            return !string.IsNullOrWhiteSpace(this.SelectedImagePath);
        }

        private async void BlurGeneralCommandExecuted(int blockSize)
        {
            if (blockSize == 3)
            {
                string bitmapPath = $"{CreateLocalFile()}{Constants.BmpExt}";

                await this.imageProvider.CreateGrayscale8Async(this.SelectedImagePath, bitmapPath);

                this.PixelsMatrix = this.imageProvider.GetBitmapPixelsMatrix(bitmapPath);
                this.GradientMatrix = this.gradientMatrixBuilder.BuildGradientMatrix(this.PixelsMatrix);
                this.LinearContrastMatrix = this.linearContraster.BuildLinearContrastMatrix(this.GradientMatrix);

                this.imageProvider.CreateBitmapFromPixelMartix(this.LinearContrastMatrix);
                this.SelectedImagePath = bitmapPath;
            }
        }

    }
}
