using GalaSoft.MvvmLight;

namespace FrequencyAnalysis
{
	public class Element<T> : ObservableObject
	{
		private T item;

		public T Item
		{
			get => this.item; 
			set
			{
				this.item = value;
				RaisePropertyChanged(nameof(Item));
			}
		}

	}
}
