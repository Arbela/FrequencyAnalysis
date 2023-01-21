using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FrequencyAnalysis.ViewModels
{
    public class CompareWindowViewModel : ViewModelBase
    {
        private CompareViewModel compareViewModel;
        private ICommand clearImagesCommand;


        public CompareWindowViewModel()
        {
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


        public ICommand ClearImagesCommand => this.clearImagesCommand ?? (this.clearImagesCommand = new RelayCommand(ClearImages));

        private void ClearImages()
        {
            this.CompareViewModel.Left = null;
            this.CompareViewModel.Right = null;
        }
    }
}
