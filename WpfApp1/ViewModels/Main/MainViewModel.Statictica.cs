using FrequencyAnalysis.Helpers;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace FrequencyAnalysis
{
    public partial class MainViewModel
    {
        private ICommand showVibrationFrequencyCommand;
        public event Action ShowChartEvent;

        public ICommand ShowVibrationFrequencyCommand
        {
            get => this.showVibrationFrequencyCommand ??
                (this.showVibrationFrequencyCommand = new RelayCommand(ExecuteShowVibrationFrequencyCommand));
        }

        private void ExecuteShowVibrationFrequencyCommand()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = false;
            fileDialog.Filter = Constants.TxtFilter;
            fileDialog.ShowDialog();

            if (fileDialog.FileNames.Any())
            {
                this.MeasuresBlurValuesPoints = ReadFile(fileDialog.FileName);
                this.ShowChartEvent?.Invoke();
            }
        }

        private IEnumerable<DataPoint> ReadFile(string filePath)
        {
            IList<DataPoint> values = new List<DataPoint>();

            using (StreamReader reader = new StreamReader(filePath))
            {
                string[] strings = reader.ReadToEnd().Split(new string[] { Environment.NewLine, " " }, StringSplitOptions.RemoveEmptyEntries);
                if (strings.Length > 0)
                {
                    for (int i = 1; i <= strings.Length; i++)
                    {
                        values.Add(new DataPoint(i, Convert.ToDouble(strings[i - 1]))); ;
                    }
                }
            }

            return values;
        }
    }
}
