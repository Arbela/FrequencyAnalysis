using FrequencyAnalysis.Helpers;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FrequencyAnalysis
{
    public partial class MainViewModel
    {
        private ICommand gradientImageMagnitude;
        private ICommand gradientImageVertical;
        private ICommand gradientImageHorizontal;

        public ICommand GradientImageMagnitude
        {
            get => this.gradientImageMagnitude ?? (this.gradientImageMagnitude = new RelayCommand(GradientImageMagnitudeExecuted, CanExecuteSelectedImagePathRelatedCommand));
        }

        public ICommand GradientImageVertical
        {
            get => this.gradientImageVertical ?? (this.gradientImageVertical = new RelayCommand(GradientImageVerticalExecuted, CanExecuteSelectedImagePathRelatedCommand));
        }

        public ICommand GradientImageHorizontal
        {
            get => this.gradientImageHorizontal ?? (this.gradientImageHorizontal = new RelayCommand(GradientImageHorizontalExecuted, CanExecuteSelectedImagePathRelatedCommand));
        }

        private async void GradientImageHorizontalExecuted()
        {
            string bitmapPath = $"{CreateLocalFile()}{Constants.BmpExt}";

            int[][] horizontalGradientMatrix = new int[0][];

            await Task.Run(async () =>
            {
                await this.imageProvider.CreateGrayscale8Async(this.SelectedImagePath, bitmapPath);

                var pixelsMatrix = this.imageProvider.GetBitmapPixelsMatrix(bitmapPath);
                horizontalGradientMatrix = this.gradientMatrixBuilder.BuildGradientMatrix(pixelsMatrix, horizontalOnly: true);
            });

            ShowMatrixEvent?.Invoke(horizontalGradientMatrix);
        }

        private async void GradientImageVerticalExecuted()
        {
            string bitmapPath = $"{CreateLocalFile()}{Constants.BmpExt}";

            int[][] verticalGradientMatrix = new int[0][];

            await Task.Run(async () =>
            {
                await this.imageProvider.CreateGrayscale8Async(this.SelectedImagePath, bitmapPath);

                var pixelsMatrix = this.imageProvider.GetBitmapPixelsMatrix(bitmapPath);
                verticalGradientMatrix = this.gradientMatrixBuilder.BuildGradientMatrix(pixelsMatrix, verticalOnly: true);
            });

            ShowMatrixEvent?.Invoke(verticalGradientMatrix);
        }

        private async void GradientImageMagnitudeExecuted()
        {
            string bitmapPath = $"{CreateLocalFile()}{Constants.BmpExt}";

            int[][] gradientMatrix = new int[0][];

            await Task.Run(async () =>
            {
                await this.imageProvider.CreateGrayscale8Async(this.SelectedImagePath, bitmapPath);

                var pixelsMatrix = this.imageProvider.GetBitmapPixelsMatrix(bitmapPath);
                gradientMatrix = this.gradientMatrixBuilder.BuildGradientMatrix(pixelsMatrix);
            });

            ShowMatrixEvent?.Invoke(gradientMatrix);
        }
    }
}
