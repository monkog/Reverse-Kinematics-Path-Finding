using System;
using System.Globalization;
using System.Windows.Data;

namespace ReverseKinematicsPathFinding.Helpers
{
    public class MoveToCenterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double width = double.Parse(value.ToString());
            double delta = double.Parse(parameter.ToString());
            return delta - width / 2.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}