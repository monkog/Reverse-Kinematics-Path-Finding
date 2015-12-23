using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ReverseKinematicsPathFinding.Helpers
{
    public class BoolToColourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value) return new SolidColorBrush(Colors.Crimson);
            return new SolidColorBrush(Colors.DarkSlateGray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}