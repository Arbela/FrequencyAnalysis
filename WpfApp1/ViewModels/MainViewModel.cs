using FrequencyAnalysis.Helpers;
using FrequencyAnalysis.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ImageFormat = System.Drawing.Imaging.ImageFormat;

namespace FrequencyAnalysis
{
    public partial class MainViewModel : ViewModelBase
    {
        #region Members
        private bool isBusy;
        private string selectedImagePath;
        private string grayscaleImagePath;
        private string importedFromFileImagePath;
        private string selectedImageFormat;
        private string mp4Path;
        private double measureBlur;
        private int[][] pixelsMatrix;
        private int[][] gradientMatrix;
        private int[][] linearContrastMatrix;
        private IEnumerable<DataPoint> measuresBlurValuesPoints;
        private ICommand exportPixelsMatrixCommand;
        private ICommand exportGradientMatrixCommand;
        private ICommand showPixelsMatrixCommand;
        private ICommand importMatrixCommand;
        private ICommand showGradientMatrixCommand;
        private ICommand showLinearContrastMatrixCommand;
        private ICommand exportLinearContrastMatrixCommand;
        private ICommand measureBlurCommand;
        private ICommand uploadVideoCommand;
        private ICommand analyzeVideoCommand;
        private ImageProvider imageProvider;
        private GradientMatrixBuilder gradientMatrixBuilder;
        private LinearContraster linearContraster;
        private ImageRetriever imageRetriever;
        private IVideoAnalyzer videoAnalyzer;

        public event Action<int[][]> ShowMatrixEvent;

        #endregion

        #region .ctor
        public MainViewModel()
        {
            this.imageProvider = new ImageProvider();
            this.gradientMatrixBuilder = new GradientMatrixBuilder();
            this.linearContraster = new LinearContraster();
            this.imageRetriever = new ImageRetriever();
        }
        #endregion

        #region Properties

        public IEnumerable<DataPoint> MeasuresBlurValuesPoints
        {
            get => this.measuresBlurValuesPoints;
            set
            {
                this.measuresBlurValuesPoints = value;
                RaisePropertyChanged(nameof(MeasuresBlurValuesPoints));
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

        public string SelectedImagePath
        {
            get => this.selectedImagePath;
            set
            {
                this.selectedImagePath = value;
                this.selectedImageFormat = ParseExtension(value);
                RaisePropertyChanged(nameof(SelectedImagePath));
            }
        }

        public string GrayscaleImagePath
        {
            get => this.grayscaleImagePath;
            set
            {
                this.grayscaleImagePath = value;
                RaisePropertyChanged(nameof(GrayscaleImagePath));
            }
        }

        public string Mp4Path
        {
            get => this.mp4Path;
            set
            {
                this.mp4Path = value;
                RaisePropertyChanged(nameof(Mp4Path));
            }
        }

        public double MeasureBlur
        {
            get => this.measureBlur;
            set
            {
                this.measureBlur = value;
                RaisePropertyChanged(nameof(MeasureBlur));
            }
        }

        public string ImportedFromFileImagePath
        {
            get => this.importedFromFileImagePath;
            set
            {
                this.importedFromFileImagePath = value;
                RaisePropertyChanged(nameof(ImportedFromFileImagePath));
            }
        }

        public int[][] PixelsMatrix
        {
            get => this.pixelsMatrix;
            set
            {
                this.pixelsMatrix = value;
                RaisePropertyChanged(nameof(PixelsMatrix));
            }
        }

        public int[][] GradientMatrix
        {
            get => this.gradientMatrix;
            set
            {
                this.gradientMatrix = value;
                RaisePropertyChanged(nameof(GradientMatrix));
            }
        }

        public int[][] LinearContrastMatrix
        {
            get => this.linearContrastMatrix;
            set
            {
                this.linearContrastMatrix = value;
                RaisePropertyChanged(nameof(LinearContrastMatrix));
            }
        }

        #endregion

        #region Commands

        public ICommand ExportPixelsMatrixCommand
        {
            get => this.exportPixelsMatrixCommand ?? (this.exportPixelsMatrixCommand = new RelayCommand(ExportPixelsMatrixCommandExecuted, IsGrayScaleImageBuild));
        }

        public ICommand ImportMatrixCommand
        {
            get => this.importMatrixCommand ?? (this.importMatrixCommand = new RelayCommand(ImportMatrixCommandExecuted));
        }

        public ICommand ExportGradientMatrixCommand
        {
            get => this.exportGradientMatrixCommand ?? (this.exportGradientMatrixCommand = new RelayCommand(ExportGradientMatrixCommandExecuted, IsGrayScaleImageBuild));
        }

        public ICommand ShowPixelsMatrixCommand
        {
            get => this.showPixelsMatrixCommand ?? (this.showPixelsMatrixCommand = new RelayCommand(ShowPixelsMatrixCommandExecuted, IsGrayScaleImageBuild));
        }

        public ICommand ShowGradientMatrixCommand
        {
            get => this.showGradientMatrixCommand ?? (this.showGradientMatrixCommand = new RelayCommand(ShowGradientMatrixCommandExecuted, IsGrayScaleImageBuild));
        }
        public ICommand ShowLinearContrastMatrixCommand
        {
            get => this.showLinearContrastMatrixCommand ?? (this.showLinearContrastMatrixCommand = new RelayCommand(ShowLinearContrastMatrixCommandExecuted, IsGrayScaleImageBuild));
        }

        public ICommand ExportLinearContrastMatrixCommand
        {
            get => this.exportLinearContrastMatrixCommand ?? (this.exportLinearContrastMatrixCommand = new RelayCommand(ExportLinearContrastMatrixCommandExecuted, () => this.GradientMatrix != null));
        }

        public ICommand MeasureBlurCommand
        {
            get => this.measureBlurCommand ?? (this.measureBlurCommand = new RelayCommand(MeasureBlurCommandExecuted, () => this.LinearContrastMatrix != null));
        }

        public ICommand UploadVideoCommand
        {
            get => this.uploadVideoCommand ?? (this.uploadVideoCommand = new RelayCommand(UploadVideoCommandExecuted));
        }

        public ICommand AnalyzeVideoCommand
        {
            get => this.analyzeVideoCommand ?? (this.analyzeVideoCommand = new RelayCommand(AnalyzeVideoCommandExecuted, () => { return this.Mp4Path != null; }));
        }

        #endregion

        #region Methods
        private async void AnalyzeVideoCommandExecuted()
        {
            string outputPath = @"C:\Users\arbela\Downloads\Output";
            this.videoAnalyzer = new VideoAnalyzer(imageRetriever, imageProvider, gradientMatrixBuilder, linearContraster, mp4Path, outputPath, outputPath);
            try
            {
                this.IsBusy = true;
                //this.MeasuresBlurValuesPoints = await this.videoAnalyzer.Analyze();
            }
            catch
            {
                
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private void UploadVideoCommandExecuted()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            fileDialog.Multiselect = false;
            fileDialog.Filter = Constants.Mp4Filter;
            fileDialog.ShowDialog();

            if (fileDialog.FileNames.Any())
            {
                this.Mp4Path = fileDialog.FileName;
            }
        }

        private void MeasureBlurCommandExecuted()
        {
            var average = this.linearContraster.CalculateAverage(this.LinearContrastMatrix);
            var dispersion = this.linearContraster.CalculateDispersion(this.LinearContrastMatrix, average);
            var variation = this.linearContraster.CalculateVariationCoefficient(average, dispersion);

            this.MeasureBlur = this.linearContraster.MeasureBlur(variation);

            //var directoryDialog = ShowSaveFileDialog(Constants.TxtFilter, Constants.TxtExtPattern);

            //if (!string.IsNullOrEmpty(directoryDialog.FileName))
            //{
            //    //ExportMeasures(directoryDialog.FileName);

            //    using (FileStream fs = File.Create(directoryDialog.FileName))
            //    {
            //        using (StreamWriter writer = new StreamWriter(fs))
            //        {
            //            for (int i = 0; i < measuresBlurValuesPoints.Count(); i++)
            //            {
            //                writer.WriteLine(/*"Image : " + (i + 1) + "   " + "Value : " + */measuresBlurValuesPoints.ElementAt(i).Y);
            //            }
            //        }
            //    }
            //}
        }

        private async void ExportLinearContrastMatrixCommandExecuted()
        {
            var directoryDialog = ShowSaveFileDialog(Constants.TxtFilter, Constants.TxtExtPattern);

            if (!string.IsNullOrEmpty(directoryDialog.FileName))
            {
                await ExportMatrix(directoryDialog.FileName, this.LinearContrastMatrix);

                ExportMatrixParameters(directoryDialog.FileName, this.LinearContrastMatrix);
            }
        }

        private void ShowGradientMatrixCommandExecuted()
        {
            ShowMatrixEvent?.Invoke(this.GradientMatrix);
        }

        private void ShowLinearContrastMatrixCommandExecuted()
        {
            ShowMatrixEvent?.Invoke(this.LinearContrastMatrix);
        }

        private void ShowPixelsMatrixCommandExecuted()
        {
            ShowMatrixEvent?.Invoke(this.PixelsMatrix);
        }

        private async void ExportGradientMatrixCommandExecuted()
        {
            var directoryDialog = ShowSaveFileDialog(Constants.TxtFilter, Constants.TxtExtPattern);

            if (!string.IsNullOrEmpty(directoryDialog.FileName))
            {
                await ExportMatrix(directoryDialog.FileName, this.GradientMatrix);

                ExportMatrixParameters(directoryDialog.FileName, this.GradientMatrix);
            }
        }

        private void ImportMatrixCommandExecuted()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            fileDialog.Multiselect = false;
            fileDialog.Filter = Constants.TxtFilter;
            fileDialog.ShowDialog();

            if (fileDialog.FileNames.Any())
            {
                string bitmapPath = CreateLocalFile($"{Constants.ImportedBmpImageName}{Constants.BmpExt}");

                this.imageProvider.CreateBitmapFromFile(fileDialog.FileName).Save(bitmapPath);
                this.ImportedFromFileImagePath = bitmapPath;
            }
        }

        private bool IsGrayScaleImageBuild()
        {
            return !string.IsNullOrWhiteSpace(this.GrayscaleImagePath);
        }

        private async void ExportPixelsMatrixCommandExecuted()
        {
            var directoryDialog = ShowSaveFileDialog(Constants.TxtFilter, Constants.TxtExtPattern);

            if (!string.IsNullOrEmpty(directoryDialog.FileName))
            {
                using (FileStream fs = File.Create(directoryDialog.FileName))
                {
                    await imageProvider.ExportBitmapPixelsMatrixAsync(this.GrayscaleImagePath, fs);
                }

                ExportMatrixParameters(directoryDialog.FileName, this.PixelsMatrix);
            }
        }

        private SaveFileDialog ShowSaveFileDialog(string filter = null, string defaultExt = null)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = filter;
            dialog.DefaultExt = defaultExt;
            dialog.ShowDialog();

            return dialog;
        }

        private void ExportMatrixParameters(string fileName, int[][] matrix)
        {
            var ouputFile = fileName.Replace(Constants.TxtExt, $"{Constants.DatExt}{Constants.TxtExt}");

            using (FileStream fs = File.Create(ouputFile))
            {
                MatrixHelper.ExportMatrixParameters(matrix, fs);
            }
        }

        private async Task ExportMatrix(string fileName, int[][] matrix)
        {
            using (FileStream fs = File.Create(fileName))
            {
                await MatrixHelper.ExportMatrixAsync(matrix, fs);
            }
        }

        private string CreateLocalFile(string fileName)
        {
            return $"{Environment.CurrentDirectory}\\{fileName}";
        }


        private void ExportMeasures(string fileName)
        {
            var average = this.linearContraster.CalculateAverage(this.LinearContrastMatrix);
            var dispersion = this.linearContraster.CalculateDispersion(this.LinearContrastMatrix, average);
            var variation = this.linearContraster.CalculateVariationCoefficient(average, dispersion);

            this.MeasureBlur = this.linearContraster.MeasureBlur(variation);

            var outputFile = fileName.Replace(Constants.TxtExt, $"_measures{Constants.TxtExt}");

            using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.Flush();
                    writer.WriteLine("Average " + average.ToString());
                    writer.WriteLine("Dispersion " + dispersion);
                    writer.WriteLine("Variation " + variation);
                    writer.WriteLine("Measure Blur " + this.MeasureBlur);
                }
            }
        }

        private string ParseExtension(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            string[] parts = value.Split('.');

             return $".{parts[parts.Length - 1].ToLower()}";
        }

        private ImageFormat StringToImageFormat(string value)
        {
            string ext = ParseExtension(value);

            switch (ext)
            {
                case Constants.JpgExt:
                    return ImageFormat.Jpeg;
                case Constants.JpegExt:
                    return ImageFormat.Jpeg;
                case Constants.PngExt:
                    return ImageFormat.Png;
                case Constants.BmpExt:
                    return ImageFormat.Bmp;
                default:
                    return null;
            }
        }

        private string ImageFormatToString(ImageFormat format)
        {
            if (format == ImageFormat.Jpeg)
                return Constants.JpegExt;
            else if (format == ImageFormat.Png)
                return Constants.PngExt;
            else if (format == ImageFormat.Bmp)
                return Constants.BmpExt;
            else
                return null;
        }


        #endregion
    }
}
