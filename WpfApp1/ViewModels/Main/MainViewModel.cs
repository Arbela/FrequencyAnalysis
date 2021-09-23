using FrequencyAnalysis.Helpers;
using FrequencyAnalysis.Interfaces;
using GalaSoft.MvvmLight;
using Microsoft.Win32;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace FrequencyAnalysis
{
    public partial class MainViewModel : ViewModelBase
    {
        #region Members
        private bool isBusy;
        private string selectedImagePath;
        private string importedFromFileImagePath;
        private string selectedImageFormat;
        private string mp4Path;
        private double measureBlur;
        private int[][] pixelsMatrix;
        private int[][] gradientMatrix;
        private int[][] linearContrastMatrix;
        private IEnumerable<DataPoint> measuresBlurValuesPoints;
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


        #region Methods

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

        private string CreateLocalFile()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "LocalImageStorage");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return $"{path}\\{123}";
        }

        private string ParseExtension(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            string[] parts = value.Split('.');

             return $".{parts[parts.Length - 1].ToLower()}";
        }

        #endregion
    }
}
