using FrequencyAnalysis.ViewModels;
using System.Windows;

namespace FrequencyAnalysis.Views
{
    /// <summary>
    /// Interaction logic for CompareImagesWindowView.xaml
    /// </summary>
    public partial class CompareImagesWindowView : Window
    {
        public CompareImagesWindowView()
        {
            InitializeComponent();
            this.DataContext = new CompareWindowViewModel();
        }
    }
}
