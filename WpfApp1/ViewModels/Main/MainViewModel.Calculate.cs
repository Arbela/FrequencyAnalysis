﻿using FrequencyAnalysis.Helpers;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FrequencyAnalysis
{
    public partial class MainViewModel
    {
        private ICommand calculateSpectralDensityCommand;

        public ICommand CalculateSpectralDensityCommand
        {
            get => this.calculateSpectralDensityCommand ?? (this.calculateSpectralDensityCommand = new RelayCommand(CalculateSpectralDensity));
        }

        private void CalculateSpectralDensity()
        {
            var average = this.linearContraster.CalculateAverage(this.GradientMatrix);
            var dispersion = this.linearContraster.CalculateDispersion(this.GradientMatrix, average);
            var variation = this.linearContraster.CalculateVariationCoefficient(average, dispersion);

           var spectralDensity = this.linearContraster.MeasureBlur(variation);
            var directoryDialog = ShowSaveFileDialog(Constants.TxtFilter, Constants.TxtExtPattern);

            if (!string.IsNullOrEmpty(directoryDialog.FileName))
            {
                using (FileStream fs = File.Create(directoryDialog.FileName))
                {
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        writer.WriteLine(spectralDensity);
                    }
                }
            }
        }
    }
}
