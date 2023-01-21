using FrequencyAnalysis.Helpers;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FrequencyAnalysis.ViewModels
{
    public class CompareViewModel : ViewModelBase
    {
        private string left;
        private string right;
        private ICommand selectLeftImageCommand;
        private ICommand selectRightImageCommand;


        public string Left
        {
            get => left; 
            set
            {
                left = value;
                RaisePropertyChanged(nameof(Left));
            }
        }

        public string Right
        {
            get => right;
            set
            { 
                right = value;
                RaisePropertyChanged(nameof(Right));
            }
        }

        public ICommand SelectLeftImageCommand => this.selectLeftImageCommand ?? (this.selectLeftImageCommand = new RelayCommand(() => this.Left = SelectImage()));

        public ICommand SelectRightImageCommand => this.selectRightImageCommand ?? (this.selectRightImageCommand = new RelayCommand(() => this.Right = SelectImage()));

        private string SelectImage()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            fileDialog.Multiselect = false;
            fileDialog.Filter = Constants.ImageFilter;
            fileDialog.ShowDialog();

            if (fileDialog.FileNames.Any())
            {
                return fileDialog.FileNames[0];
            }

            return null;
        }
    }
}
