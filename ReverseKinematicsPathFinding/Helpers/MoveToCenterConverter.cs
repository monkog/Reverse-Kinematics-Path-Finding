using System;
using System.Globalization;
using System.Windows.Data;

namespace ReverseKinematicsPathFinding.Helpers
{
    public class MoveToCenterConverter : IMultiValueConverter
    {
	    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			double objectSize = double.Parse(values[0].ToString());
			double screenSize = double.Parse(values[1].ToString());
			return (screenSize - objectSize) / 2.0;
		}

	    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	    {
		    throw new NotImplementedException();
	    }
    }
}