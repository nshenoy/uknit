using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;

namespace uknit.Helpers.Converters
{
	public class DoubleToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			double doubleToConvert = (double)value;
			string convertedString = doubleToConvert.ToString("F2");

			int index = convertedString.IndexOf('.');
			if(convertedString.Substring(index+1) == "00")
			{
				convertedString = convertedString.Substring(0, index);
			}

			return convertedString;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
