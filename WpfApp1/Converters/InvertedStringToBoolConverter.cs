using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Data;

namespace FrequencyAnalysis.Converters
{
    public class InvertedStringToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrWhiteSpace(value as string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
