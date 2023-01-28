using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FrequencyAnalysis.Helpers;
using FrequencyAnalysis.Providers;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace FrequencyAnalysis.ViewModels
{
    public class CompareWindowViewModel : ViewModelBase
    {
        private CompareViewModel compareViewModel;
        private ICommand clearImagesCommand;
        private ICommand convertToGrayscaleCommand;
        private RelayCommand<int> blurGeneralCommand;
        private RelayCommand<int> blurVerticalCommand;
        private RelayCommand<int> blurHorizontalCommand;
        private DialogService dialogService;
        private ImageProvider imageProvider;
        private GradientMatrixBuilder gradientMatrixBuilder;
        private LinearContraster linearContraster;
        private bool isBusy;

        public CompareWindowViewModel()
        {
            this.imageProvider= new ImageProvider();
            this.gradientMatrixBuilder = new GradientMatrixBuilder();
            this.linearContraster= new LinearContraster();
            this.dialogService = new DialogService();
            this.CompareViewModel = new CompareViewModel();
        }

        public CompareViewModel CompareViewModel
        {
            get => this.compareViewModel;
            set
            {
                this.compareViewModel = value;
                RaisePropertyChanged(nameof(CompareViewModel));
            }
        }
        public bool IsBusy
        {
            get => this.isBusy;
            set
            {
                this.isBusy = value;
                RaisePropertyChanged(nameof(IsBusy));
            }
        }


        public ICommand ClearImagesCommand 
            => this.clearImagesCommand ?? (this.clearImagesCommand = new RelayCommand(ClearImages));
        public ICommand ConvertToGrayscaleCommand
            => this.convertToGrayscaleCommand ??
            (this.convertToGrayscaleCommand = new RelayCommand(ConvertToGrayscaleCommandExecuted));


        public RelayCommand<int> BlurGeneralCommand 
            => this.blurGeneralCommand ??
            (this.blurGeneralCommand = new RelayCommand<int>(async _ => await ExecuteBlur(true, true)));


        public RelayCommand<int> BlurVerticalCommand =>
            this.blurVerticalCommand ??
            (this.blurVerticalCommand = new RelayCommand<int>(async _ => await ExecuteBlur(true, false)));


        public RelayCommand<int> BlurHorizontalCommand =>
            this.blurHorizontalCommand ?? 
            (this.blurHorizontalCommand = new RelayCommand<int>(async _ => await ExecuteBlur(false, true)));

        private void ClearImages()
        {
            this.CompareViewModel.Left = null;
            this.CompareViewModel.Right = null;
        }

        private async void ConvertToGrayscaleCommandExecuted()
        {
            var directoryDialogLeft = dialogService.ShowSaveFileDialog(
                Constants.BmpFilter,
                Constants.BmpExtPattern, 
                "Save Left Image as");

            if (string.IsNullOrEmpty(directoryDialogLeft.FileName)) 
                return;
            string bitmapPathLeft = directoryDialogLeft.FileName;

            var directoryDialogRight = dialogService.ShowSaveFileDialog(
                Constants.BmpFilter, 
                Constants.BmpExtPattern, 
                "Save Right Image as");

            if (string.IsNullOrEmpty(directoryDialogRight.FileName))
                return;
            string bitmapPathRight = directoryDialogRight.FileName;

            try
            {
                this.IsBusy = true;
                await Task.WhenAll(new Task[]
                {
                    this.imageProvider.CreateGrayscale8Async(this.CompareViewModel.Left, bitmapPathLeft),
                    this.imageProvider.CreateGrayscale8Async(this.CompareViewModel.Right, bitmapPathRight)
                });
                this.CompareViewModel.Left = bitmapPathLeft;
                this.CompareViewModel.Right = bitmapPathRight;

            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private async Task ExecuteBlur(bool isVertical, bool isHorizontal)
        {
            if (string.IsNullOrWhiteSpace(this.CompareViewModel.Left) || string.IsNullOrWhiteSpace(this.CompareViewModel.Right))
            {
                return;
            }

            var directoryDialogLeft = dialogService.ShowSaveFileDialog(
                Constants.BmpFilter,
                Constants.BmpExtPattern,
                "Save Left Image as");

            if (string.IsNullOrEmpty(directoryDialogLeft.FileName))
                return;
            string bitmapPathLeft = directoryDialogLeft.FileName;

            var directoryDialogRight = dialogService.ShowSaveFileDialog(
                Constants.BmpFilter,
                Constants.BmpExtPattern,
                "Save Right Image as");

            if (string.IsNullOrEmpty(directoryDialogRight.FileName))
                return;
            string bitmapPathRight = directoryDialogRight.FileName;

            this.IsBusy = true;
            int[][] matrixLeft = new int[0][];
            int[][] matrixRight = new int[0][];

            try
            {
                await Task.WhenAll(new Task[]
                {
                    Task.Run(() =>
                    {
                        matrixLeft = ProcessImage(this.CompareViewModel.Left, bitmapPathLeft, isVertical, isHorizontal);
                    }),
                    Task.Run(() =>
                    {
                        matrixRight = ProcessImage(this.CompareViewModel.Right, bitmapPathRight, isVertical, isHorizontal);
                    })
                });

                this.CompareViewModel.Left = bitmapPathLeft;
                this.CompareViewModel.Right = bitmapPathRight;
            }
            finally
            {
                this.IsBusy = false;
            }

            if (MessageBox.Show(
                    "Save Form Parameters to .txt and .csv files?",
                    "Save Form Parameters",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var formParameterLeft = MeasureFormParameter(matrixLeft);
                var formParameterRight = MeasureFormParameter(matrixRight);

                var dialog = dialogService.ShowSaveFileDialog();

                if (string.IsNullOrWhiteSpace(dialog.FileName))
                {
                    return;
                }

                ExportToTxt(formParameterLeft, formParameterRight, dialog.FileName);
                ExportToCsv(formParameterLeft, formParameterRight, dialog.FileName);
            }
        }

        private int[][] ProcessImage(string source, string newBitmap, bool isVertical, bool isHorizontal)
        {
            var pixelsMatrix = this.imageProvider.GetBitmapPixelsMatrix(source);
            var gradientMatrix = this.gradientMatrixBuilder.BuildGradientMatrix(pixelsMatrix, isVertical, isHorizontal);
            var linearContrastMatrix = this.linearContraster.BuildLinearContrastMatrix(gradientMatrix);

            var blurBitmap = this.imageProvider.CreateBitmapFromPixelMartix(linearContrastMatrix);
            blurBitmap.Save(newBitmap, ImageFormat.Bmp);

            return linearContrastMatrix;

        }

        private double MeasureFormParameter(int[][] linearContrastMatrix)
        {
            var average = this.linearContraster.CalculateAverage(linearContrastMatrix);
            var dispersion = this.linearContraster.CalculateDispersion(linearContrastMatrix, average);
            var variation = this.linearContraster.CalculateVariationCoefficient(average, dispersion);

            return this.linearContraster.MeasureFormParameter(variation);
        }

        private void ExportToTxt(double left, double right, string path)
        {
            string fileName = $"{path}{Constants.TxtExt}";

            using (FileStream fs = File.Create(fileName))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.WriteLine($"Left: {left}");
                    writer.WriteLine($"Right: {right}");
                }
            }
        }

        private async void ExportToCsv(double left, double right, string path)
        {
            string fileName = $"{path}{Constants.CsvExt}";
            CSVWriter csvWriter = new CSVWriter(fileName);
            await csvWriter.Write(new Similarity
            {
                Left = left,
                Right = right,
            });
        }

        internal void Dispose()
        {
            CompareViewModel.Left = null;
            CompareViewModel.Right = null;
        }

        class Similarity
        {
            public double Left { get; set; }
            public double Right { get; set; }
        }
    }
}
