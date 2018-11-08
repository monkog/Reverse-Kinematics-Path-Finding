using System;
using System.Globalization;
using System.Windows.Data;

namespace ReverseKinematicsPathFinding.Helpers
{
	public class AdditionConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			double sum = 0;
			foreach (var value in values)
			{
				sum += double.Parse(value.ToString());
			}

			return sum;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}