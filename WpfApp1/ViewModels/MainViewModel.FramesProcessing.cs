using FrequencyAnalysis.Helpers;
using GalaSoft.MvvmLight.Command;
using Ookii.Dialogs.Wpf;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace FrequencyAnalysis
{
    public partial class MainViewModel
    {
        private ICommand saveBlurGeneralSequenceCommand;
        private ICommand saveBlurVerticalSequenceCommand;
        private ICommand saveBlurHorizontalSequenceCommand;

        public ICommand SaveBlurGeneralSequenceCommand
        {
            get => this.saveBlurGeneralSequenceCommand ??
                (this.saveBlurGeneralSequenceCommand = new RelayCommand(ExecuteSaveBlurGeneralSequenceCommand));
        }

        public ICommand SaveBlurVerticalSequenceCommand
        {
            get => this.saveBlurVerticalSequenceCommand ??
                (this.saveBlurVerticalSequenceCommand = new RelayCommand(ExecuteSaveBlurVerticalSequenceCommand));
        }

        public ICommand SaveBlurHorizontalSequenceCommand
        {
            get => this.saveBlurHorizontalSequenceCommand ??
                (this.saveBlurHorizontalSequenceCommand = new RelayCommand(ExecuteSaveBlurHorizontalSequenceCommand));
        }

        private async void ExecuteSaveBlurHorizontalSequenceCommand()
        {
            await SaveBlurSequence(verticalOnly: false, horizontalOnly: true);
        }

        private async void ExecuteSaveBlurVerticalSequenceCommand()
        {
            await SaveBlurSequence(verticalOnly: true, horizontalOnly: false);
        }

        private async void ExecuteSaveBlurGeneralSequenceCommand()
        {
            await SaveBlurSequence(verticalOnly: false, horizontalOnly: false);
        }

        private async Task SaveBlurSequence(bool verticalOnly, bool horizontalOnly)
        {
            var dlg = new VistaFolderBrowserDialog();
            if (dlg.ShowDialog() == true && !string.IsNullOrWhiteSpace(dlg.SelectedPath) && Directory.Exists(dlg.SelectedPath))
            {
                var videoAnalyzer = new VideoAnalyzer(imageRetriever, imageProvider, gradientMatrixBuilder, linearContraster, mp4Path, dlg.SelectedPath, dlg.SelectedPath);
                string convertedImagesPath = await videoAnalyzer.ConvertToGrayScale();

                var sequence = await videoAnalyzer.Analyze(convertedImagesPath, verticalOnly, horizontalOnly);

                await Task.Yield();
                ExportBlurSequence(sequence, convertedImagesPath);
            }
        }

        private void ExportBlurSequence(IEnumerable<DataPoint> measuresBlurValuesPoints, string path)
        {
            string fileName = $"{path}\\Sequence{Constants.TxtExt}";

            using (FileStream fs = File.Create(fileName))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    for (int i = 0; i < measuresBlurValuesPoints.Count(); i++)
                    {
                        writer.WriteLine(measuresBlurValuesPoints.ElementAt(i).Y);
                    }
                }
            }
        }
    }
}
