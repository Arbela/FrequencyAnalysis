using FrequencyAnalysis.Helpers;
using FrequencyAnalysis.Models;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.IO;
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
            await GradientImageVisualization(false, true);
        }

        private async void GradientImageVerticalExecuted()
        {
            await GradientImageVisualization(true, false);
        }

        private async void GradientImageMagnitudeExecuted()
        {
            await GradientImageVisualization(true, true);
        }

        private async Task GradientImageVisualization(bool verticalOnly, bool horizontalOnly)
        {
            string bitmapPath = $"{CreateLocalFile()}{Constants.BmpExt}";

            int[][] linearMatrix = new int[0][];

            try
            {
                this.IsBusy = true;

                await Task.Run(async () =>
                {
                    await this.imageProvider.CreateGrayscale8Async(this.SelectedImagePath, bitmapPath);

                    var pixelsMatrix = this.imageProvider.GetBitmapPixelsMatrix(bitmapPath);
                    var gradientMatrix = this.gradientMatrixBuilder.BuildGradientMatrix(pixelsMatrix, verticalOnly, horizontalOnly);
                    linearMatrix = this.linearContraster.BuildLinearContrastMatrix(gradientMatrix);
                });
            }
            finally
            {
                this.IsBusy = false;
                if (File.Exists(bitmapPath))
                {
                    File.Delete(bitmapPath);
                }
            }

            string title = string.Empty;
            if (verticalOnly && horizontalOnly)
                title = "Magnitude Matrix";
            else if (verticalOnly)
                title = "Vertical Magnitude Matrix";
            else if (horizontalOnly)
                title = "Horizontal Magnitude Matrix";


            ShowMatrixEvent?.Invoke(this, new MatrixVisualizationEventArgs { Source = linearMatrix, Title = title });
        }

    }
}
