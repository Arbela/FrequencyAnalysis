using GalaSoft.MvvmLight;
using System;
using System.Text;

namespace FrequencyAnalysis
{
    public class MatrixViewModel<T> : ViewModelBase
    {
        private string stringMatrix;
        private string title;
        private Element<T>[][] matrixItems;

        public MatrixViewModel(T[][] source, string title)
        {
            //BuildMatrix(source);
            this.Title = title;
            BuildStringMatrix(source);
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

        public string StringMatrix
        {
            get => this.stringMatrix;
            set
            {
                this.stringMatrix = value;
                RaisePropertyChanged(nameof(StringMatrix));
            }
        }

        public string Title
        {
            get => this.title;
            set
            {
                this.title = value;
                RaisePropertyChanged(nameof(Title));
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

        private void BuildStringMatrix(T[][] source)
        {
            StringBuilder strb = new StringBuilder();

            for (int i = 0; i < source.Length; i++)
            {
                strb.AppendLine();
                for (int j = 0; j < source[i].Length; j++)
                {
                    strb.Append(source[i][j]);
                    strb.Append(" ");
                }
            }

            this.StringMatrix = strb.ToString();
        }
    }
}
