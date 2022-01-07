using FrequencyAnalysis.Helpers;
using GalaSoft.MvvmLight.CommandWpf;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace FrequencyAnalysis
{
    public partial class MainViewModel
    {
        private RelayCommand<int> blurGeneralCommand;
        private RelayCommand<int> blurVerticalCommand;
        private RelayCommand<int> blurHorizontalCommand;
        private RelayCommand convertToGrayscaleCommand;


        public RelayCommand<int> BlurGeneralCommand
        {
            get => this.blurGeneralCommand ?? (this.blurGeneralCommand = new RelayCommand<int>(async _ => await ExecuteBlur(true, true)));
        }

        public RelayCommand<int> BlurVerticalCommand =>
            this.blurVerticalCommand ?? (this.blurVerticalCommand = new RelayCommand<int>(async _ => await ExecuteBlur(true, false)));


        public RelayCommand<int> BlurHorizontalCommand =>
            this.blurHorizontalCommand ?? (this.blurHorizontalCommand = new RelayCommand<int>(async _ => await ExecuteBlur(false, true)));

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

        private async Task ExecuteBlur(bool isVertical, bool isHorizontal)
        {
            if (string.IsNullOrWhiteSpace(this.SelectedImagePath)) return;

            var directoryDialog = ShowSaveFileDialog(Constants.BmpFilter, Constants.BmpExtPattern);

            if (string.IsNullOrEmpty(directoryDialog.FileName)) return;

            try
            {
                this.IsBusy = true;

                string bitmapPath = directoryDialog.FileName;

                await Task.Run(() =>
                {
                    this.PixelsMatrix = this.imageProvider.GetBitmapPixelsMatrix(this.SelectedImagePath);
                    this.GradientMatrix = this.gradientMatrixBuilder.BuildGradientMatrix(this.PixelsMatrix, isVertical, isHorizontal);
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
