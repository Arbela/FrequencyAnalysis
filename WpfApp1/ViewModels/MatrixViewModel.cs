using GalaSoft.MvvmLight;

namespace FrequencyAnalysis
{
    public class MatrixViewModel<T> : ViewModelBase
    {
        private Element<T>[][] matrixItems;

        public MatrixViewModel(T[][] source)
        {
            BuildMatrix(source);
        }

        public Element<T>[][] MatrixItems
        {
            get => this.matrixItems;
            set
            {
                this.matrixItems = value;
                RaisePropertyChanged(nameof(MatrixItems));
            }
        }

        private void BuildMatrix(T[][] source)
        {
            this.MatrixItems = new Element<T>[source.Length][];
            for (int i = 0; i < source.Length; i++)
            {
                this.MatrixItems[i] = new Element<T>[source[i].Length];
                for (int j = 0; j < source[i].Length; j++)
                {
                    this.MatrixItems[i][j] = new Element<T>() { Item = source[i][j] };
                }
            }
        }
    }
}
