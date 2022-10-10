using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Pyanulis.RayTracing.Converters
{
    internal class ImageStrategyConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string? parameterString = parameter?.ToString();
            if (parameterString == null)
                return Binding.DoNothing;

            if (Enum.IsDefined(value.GetType(), value) == false)
                return Binding.DoNothing;

            object parameterValue = Enum.Parse(value.GetType(), parameterString);

            return parameterValue.Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string? parameterString = parameter?.ToString();
            if (parameterString == null)
                return Binding.DoNothing;

            if ((bool)value)
            {
                return Enum.Parse(targetType, parameterString);
            }
            return Binding.DoNothing;
        }
        #endregion
    }
}
